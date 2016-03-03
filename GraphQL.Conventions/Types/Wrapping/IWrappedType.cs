using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Types;

namespace GraphQL.Conventions.Types.Wrapping
{
    interface IWrappedType
    {
        Entity Entity { get; }

        GraphType Type { get; }

        object Wrap(object value);

        object Unwrap(object value);

        object FromDictionary(Dictionary<string, object> dictionary);
    }
}
