using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddViewsApplicationParts(this IMvcCoreBuilder mvcBuilder)
        {
            var viewAssemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Views.dll")
                .Select(Assembly.LoadFrom)
                .ToArray();

            return mvcBuilder.AddViewsApplicationParts(viewAssemblies);
        }

        public static IMvcCoreBuilder AddViewsApplicationParts(this IMvcCoreBuilder mvcBuilder, Assembly[] viewAssemblies)
        {
            foreach (var assembly in viewAssemblies)
                mvcBuilder.AddApplicationPart(assembly);

            return mvcBuilder;
        }
    }
}
