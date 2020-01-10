using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension class for <see cref="IMvcCoreBuilder"/>
    /// </summary>
    public static class IMvcCoreBuilderExtensions
    {
        /// <summary>
        /// Add all *.Views.dll files found in the base directory as application parts
        /// </summary>
        /// <param name="mvcBuilder">The builder the application parts are added to.</param>
        /// <returns>The <paramref name="mvcBuilder"/>.</returns>
        public static IMvcCoreBuilder AddViewsApplicationParts(this IMvcCoreBuilder mvcBuilder)
        {
            var viewAssemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Views.dll")
                .Select(Assembly.LoadFrom)
                .ToArray();

            return mvcBuilder.AddViewsApplicationParts(viewAssemblies);
        }

        /// <summary>
        /// Add multiple assemblies as application parts to the <paramref name="mvcBuilder"/>
        /// </summary>
        /// <param name="mvcBuilder">The builder the application parts are added to.</param>
        /// <param name="viewAssemblies">The <see cref="Assembly"/>'s to add as application parts.</param>
        /// <returns>The <paramref name="mvcBuilder"/>.</returns>
        public static IMvcCoreBuilder AddViewsApplicationParts(this IMvcCoreBuilder mvcBuilder, Assembly[] viewAssemblies)
        {
            foreach (var assembly in viewAssemblies)
                mvcBuilder.AddApplicationPart(assembly);

            return mvcBuilder;
        }
    }
}
