using System;
using System.Collections.Generic;

namespace Skyhop.Mail.Abstractions
{
    public interface IModelIdentifierLister
    {
        IEnumerable<(Type ModelType, string Identifier)> ModelIdentifiers { get; }
    }
}
