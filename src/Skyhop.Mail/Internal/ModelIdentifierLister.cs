using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyhop.Mail.Internal
{
    internal class ModelIdentifierLister : IModelIdentifierLister
    {
        public IEnumerable<(Type ModelType, string Identifier)> ModelIdentifiers { get; }

        public ModelIdentifierLister(ApplicationPartManager applicationPartManager)
        {
            ModelIdentifiers = _listViews(applicationPartManager).ToArray();
        }

        private IEnumerable<(Type Model, string Identifier)> _listViews(ApplicationPartManager applicationPartManager)
        {
            var feature = new ViewsFeature();
            applicationPartManager.PopulateFeature(feature);
            var views = feature.ViewDescriptors
                .Select(x => _getModelType(x.Item))
                .Where(x => x.HasModel)
                .Select(x => (x.Model, x.Identifier));

            foreach (var (model, identifier) in views)
                if (model is object && identifier is object)
                    yield return (model, identifier);
        }

        private (bool HasModel, string? Identifier, Type? Model) _getModelType(RazorCompiledItem item)
        {
            var (hasModel, model) = _getModelType(item.Type);
            return (hasModel, item.Identifier, model);
        }

        private (bool HasModel, Type? Model) _getModelType(Type type)
        {
            if (type.BaseType == null || type == typeof(object))
                return (false, default);

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(RazorPage<>))
                {
                    var genericArguments = type.GetGenericArguments();
                    if (genericArguments.Length == 1)
                        return (true, genericArguments[0]);
                }
            }
            return _getModelType(type.BaseType);
        }
    }
}
