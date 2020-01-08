using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skyhop.Mail;
using Skyhop.Mail.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMailDispatcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> mailDispatcherOptionsBuilder)
        {
            return AddMailDispatcher(serviceCollection, mailDispatcherOptionsBuilder, mvcBuilder =>
            {
                var viewAssemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Views.dll")
                    .Select(Assembly.LoadFrom)
                    .ToArray();

                foreach (var assembly in viewAssemblies)
                {
                    mvcBuilder.AddApplicationPart(assembly);
                }
            });
        }

        public static IServiceCollection AddMailDispatcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> mailDispatcherOptionsBuilder, Action<IMvcCoreBuilder>? mvcCoreBuilderAction)
        {
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            serviceCollection.AddSingleton(diagnosticSource);
            serviceCollection.AddSingleton<DiagnosticSource>(diagnosticSource);

            serviceCollection.TryAddSingleton<IWebHostEnvironment, WebHostEnvironmentShell>();

            serviceCollection.AddSingleton<RazorViewToStringRenderer>();

            serviceCollection.AddSingleton<IModelIdentifierLister, ModelIdentifierLister>();

            serviceCollection.Configure(mailDispatcherOptionsBuilder);
            serviceCollection.AddSingleton<MailDispatcher>();

            var mvcCoreBuilder = serviceCollection.AddMvcCore()
                .AddViews()
                .AddRazorViewEngine();

            mvcCoreBuilderAction?.Invoke(mvcCoreBuilder);

            return serviceCollection;
        }
    }
}
