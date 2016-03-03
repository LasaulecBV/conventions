using System;
using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Reflection;
using GraphQL.Types;

namespace GraphQL.Conventions.Types.Wrapping
{
    class WrappedType : IWrappedType
    {
        public WrappedType(Entity entity)
        {
            Entity = entity;
            Type = entity.GraphType;
        }

        public Entity Entity { get; protected set; }

        public GraphType Type { get; protected set; }

        public virtual object Unwrap(object value)
        {
            return Conversion.ConvertImplicity(
                value,
                Entity.OriginalType,
                Entity.TypeRepresentation);
        }

        public virtual object Wrap(object value)
        {
            return Conversion.ConvertImplicity(
                value,
                Entity.TypeRepresentation,
                Entity.OriginalType);
        }

        public object FromDictionary(Dictionary<string, object> dictionary)
        {
            throw new NotImplementedException();
        }
    }
}
