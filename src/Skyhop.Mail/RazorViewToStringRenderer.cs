using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    // Code from: https://github.com/aspnet/Entropy/blob/master/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs

    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = _getActionContext();
            var view = _findView(actionContext, viewName);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private IView _findView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext _getActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        public static RazorViewToStringRenderer GetRenderer()
        {
            var services = new ServiceCollection();

            var appDirectory = Directory.GetCurrentDirectory();

            var webhostBuilder = new WebHostBuilder().ConfigureServices(services =>
            {
                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
                });

                services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

                var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
                services.AddSingleton<DiagnosticListener>(diagnosticSource);
                services.AddSingleton<DiagnosticSource>(diagnosticSource);

                services.AddLogging();

                var mvcBuilder = services.AddRazorPages();

                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Views.dll")
                    .Select(file => Assembly.LoadFrom(file))
                    .ToList()
                    .ForEach(assembly => mvcBuilder.AddApplicationPart(assembly));
                
                services.AddSingleton<RazorViewToStringRenderer>();
            }).UseStartup<Startup>().Build();

            return webhostBuilder.Services.GetRequiredService<RazorViewToStringRenderer>();
        }

        private class Startup
        {
            public void Configure()
            {

            }
        }
    }
}
