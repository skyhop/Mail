using System;
using System.Collections.Generic;

namespace Skyhop.Mail.Abstractions
{
    /// <summary>
    /// Class that lists Models and the view identifiers that belong to them.
    /// </summary>
    public interface IModelIdentifierLister
    {
        /// <summary>
        /// A collection of ModelTyps and the view identifiers that belong to them.
        /// </summary>
        IEnumerable<(Type ModelType, string Identifier)> ModelIdentifiers { get; }
    }
}
