using System;
using System.Collections.Generic;

namespace Skyhop.Mail
{
    public interface IModelIdentifierLister
    {
        IEnumerable<(Type ModelType, string Identifier)> ModelIdentifiers { get; }
    }
}
