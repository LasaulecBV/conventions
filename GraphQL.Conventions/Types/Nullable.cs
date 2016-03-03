using System;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Types
{
    internal class Nullable : WrappedType
    {
        public Nullable(Entity entity)
            : base(entity)
        {
            if (entity.TypeRepresentation.IsGenericType &&
                entity.TypeRepresentation.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var innerEntity = entity.Construct(entity.TypeRepresentation.GenericTypeArguments.First());
                var type = innerEntity.GraphType.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
                {
                    Type = (GraphType)Activator.CreateInstance(type.GenericTypeArguments.First());
                    Entity.OriginalType = innerEntity.OriginalType;
                    Entity.TypeRepresentation = innerEntity.TypeRepresentation;

                    if (Entity.OriginalType.IsValueType)
                    {
                        Entity.OriginalType = typeof(Nullable<>).MakeGenericType(Entity.OriginalType);
                    }

                    if (Entity.TypeRepresentation.IsValueType)
                    {
                        Entity.TypeRepresentation = typeof(Nullable<>).MakeGenericType(Entity.TypeRepresentation);
                    }
                }
                else
                {
                    Type = innerEntity.GraphType;
                }
            }
            else
            {
                Entity = entity.Construct(entity.TypeRepresentation);
                Type = Entity.GraphType;
            }
        }

        public override object Wrap(object value)
        {
            return base.Wrap(value);
        }

        public override object Unwrap(object value)
        {
            return base.Unwrap(value);
        }
    }
}
