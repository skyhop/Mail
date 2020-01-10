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
    /// <summary>
    /// Extension class for <see cref="IServiceCollection"/>
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add the <see cref="MailDispatcher"/> and needed dependencies to the <paramref name="serviceCollection"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the dependencies to.</param>
        /// <param name="mailDispatcherOptionsBuilder">An <see cref="Action{MailDispatcherOptions}"/> that is used to configure the options.</param>
        /// <param name="mvcCoreBuilderAction">An optional <see cref="Action{IMvcCoreBuilder}"/> used to configure extra mvc core options.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddMailDispatcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> mailDispatcherOptionsBuilder, Action<IMvcCoreBuilder>? mvcCoreBuilderAction = default)
        {
            // Renderer + internal dependencies
            serviceCollection.AddSingleton<RazorViewToStringRenderer>();
            serviceCollection.AddSingleton<IModelIdentifierLister, ModelIdentifierLister>();
            serviceCollection.AddSingleton<MailDispatcher>();
            serviceCollection.Configure(mailDispatcherOptionsBuilder);

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
