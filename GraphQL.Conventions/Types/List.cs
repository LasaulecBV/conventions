using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Types
{
    internal class List : WrappedType
    {
        public List(Entity entity)
            : base(entity)
        {
            if (!entity.TypeRepresentation.IsGenericType ||
                entity.TypeRepresentation.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new ArgumentException(
                    "List wrapper can only be applied to List<>",
                    nameof(entity.TypeRepresentation));
            }

            var baseTypeEntity = entity.Construct(entity.TypeRepresentation.GenericTypeArguments.First());
            var baseGraphType = baseTypeEntity.GraphType ?? entity.GraphType;
            var type = typeof(ListGraphType<>).MakeGenericType(baseGraphType.GetType());

            Entity = baseTypeEntity;
            Type = (GraphType)Activator.CreateInstance(type);
        }

        public override object Wrap(object value)
        {
            var list = value as IList;
            var genericListType = typeof(List<>).MakeGenericType(Entity.WrappedType.Entity.OriginalType);
            var result = (IList)Activator.CreateInstance(genericListType);
            foreach (var item in list)
            {
                var wrappedItem = Entity.WrappedType.Wrap(item);
                result.Add(wrappedItem);
            }
            return result;
        }

        public override object Unwrap(object value)
        {
            var list = value as IList;
            var genericListType = typeof(List<>).MakeGenericType(Entity.WrappedType.Entity.TypeRepresentation);
            var result = (IList)Activator.CreateInstance(genericListType);
            foreach (var item in list)
            {
                var unwrappedItem = Entity.WrappedType.Unwrap(item);
                result.Add(unwrappedItem);
            }
            return result;
        }
    }
}
