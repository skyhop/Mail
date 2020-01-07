using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SkyHop.Mail;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skyhop.Mail
{
    public static class Extensions
    {
        public static async Task<string> GetViewForModel<T>(this RazorViewToStringRenderer renderer, T model)
        {
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Views.dll");

            // ToDo: Find a way to cache these loaded assemblies.
            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file);

                var identifier = assembly
                    .ExportedTypes
                    .SingleOrDefault(q => q.IsSubclassOf(typeof(RazorPage<T>)))
                    ?.GetCustomAttribute<RazorSourceChecksumAttribute>()
                    ?.Identifier;

                if (identifier != null) return await renderer.RenderViewToStringAsync(identifier, model);
            }

            return null;
        }

        public static IServiceCollection AddMailDispatpcher(this IServiceCollection serviceCollection, Action<MailDispatcherOptions> optionsBuilder)
        {
            var renderer = RazorViewToStringRenderer.GetRenderer();

            var options = new MailDispatcherOptions();

            optionsBuilder.Invoke(options);

            var dispatcher = new MailDispatcher(renderer, options);
            serviceCollection.AddSingleton(dispatcher);
            return serviceCollection;
        }
    }
}
