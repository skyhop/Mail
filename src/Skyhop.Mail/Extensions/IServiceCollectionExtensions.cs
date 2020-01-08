using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skyhop.Mail;
using Skyhop.Mail.Abstractions;
using Skyhop.Mail.Internal;
using Skyhop.Mail.Options;
using System;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMailDispatcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> mailDispatcherOptionsBuilder, ServiceLifetime mailDispatcherLifetime = ServiceLifetime.Singleton)
            => AddMailDispatcher(serviceCollection, mailDispatcherOptionsBuilder, mvcBuilder => mvcBuilder.AddViewsApplicationParts(), mailDispatcherLifetime);

        public static IServiceCollection AddMailDispatcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> mailDispatcherOptionsBuilder, Action<IMvcCoreBuilder>? mvcCoreBuilderAction, ServiceLifetime mailDispatcherLifetime = ServiceLifetime.Singleton)
        {
            // Renderer + internal dependencies
            serviceCollection.AddSingleton<RazorViewToStringRenderer>();
            serviceCollection.AddSingleton<IModelIdentifierLister, ModelIdentifierLister>();
            serviceCollection.Configure(mailDispatcherOptionsBuilder);

            // Use the lifetime the end-user chosen, in case their IMailSender implementation needs to be scoped
            serviceCollection.Add(new ServiceDescriptor(typeof(MailDispatcher), typeof(MailDispatcher), mailDispatcherLifetime));

            // Try add if not already added needed Razor dependencies
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            serviceCollection.TryAddSingleton(diagnosticSource);
            serviceCollection.TryAddSingleton<DiagnosticSource>(diagnosticSource);
            serviceCollection.TryAddSingleton<IWebHostEnvironment, WebHostEnvironmentShell>();

            // Minimum needed MVC
            var mvcCoreBuilder = serviceCollection.AddMvcCore()
                .AddViews()
                .AddRazorViewEngine();
            mvcCoreBuilderAction?.Invoke(mvcCoreBuilder);

            return serviceCollection;
        }
    }
}
