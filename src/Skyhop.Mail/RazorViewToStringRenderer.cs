using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Skyhop.Mail.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    /// <summary>
    /// An independent Razor view to string renderer
    /// </summary>
    /// <remarks>
    /// Based upon code from: https://github.com/aspnet/Entropy/blob/93ee2cf54eb700c4bf8ad3251f627c8f1a07fb17/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
    /// </remarks>
    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IModelIdentifierLister _viewLister;

        /// <summary>
        /// Constructor for <see cref="RazorViewToStringRenderer"/>
        /// </summary>
        /// <param name="viewEngine">View engine used to find views</param>
        /// <param name="tempDataProvider">TempDataProvider used for <seealso cref="ViewContext"/> construction</param>
        /// <param name="serviceScopeFactory">The <seealso cref="IServiceScopeFactory"/> used to create a scope to resolve web/razor services used for view rendering</param>
        /// <param name="viewLister">The <seealso cref="IModelIdentifierLister"/> used to make model to view identifier lookups</param>
        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceScopeFactory serviceScopeFactory,
            IModelIdentifierLister viewLister)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceScopeFactory = serviceScopeFactory;
            _viewLister = viewLister;
        }

        /// <summary>
        /// Render a model to a html <see cref="string"/>
        /// </summary>
        /// <typeparam name="T">The type of model to render</typeparam>
        /// <param name="model">The model to render</param>
        /// <returns>The rendered string</returns>
        /// <remarks>Returns null if a suitable views could not be found.</remarks>
        public async Task<string?> RenderViewForModel<T>(T model)
        {
            var views = _viewLister.ModelIdentifiers;

            foreach (var (modelType, identifier) in views)
                if (modelType == typeof(T))
                    return await RenderViewToStringAsync(identifier, model);

            return null;
        }

        /// <summary>
        /// Renders a view based on the view identifier
        /// </summary>
        /// <typeparam name="TModel">The model type used in the view</typeparam>
        /// <param name="viewName">The view identifier</param>
        /// <param name="model">The model used in the view</param>
        /// <returns>The rendered view</returns>
        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            // Scope is needed for the scoped services
            using var scope = _serviceScopeFactory.CreateScope();

            var actionContext = _getActionContext(scope.ServiceProvider);

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

        private ActionContext _getActionContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
