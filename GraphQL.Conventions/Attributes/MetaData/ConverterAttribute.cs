using System;
using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.MetaData
{
    class ConverterAttribute : MetaDataAttributeBase
    {
        public const int DefaultOrder = OverrideOrder - 1;
        public const int OverrideOrder = int.MaxValue;

        protected readonly Type _wrapper;

        public ConverterAttribute(Type wrapper)
        {
            _wrapper = wrapper ?? typeof(PlainWrappedType);
            Order = OverrideOrder;
        }

        internal ConverterAttribute()
            : this(null)
        {
            Order = DefaultOrder;
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (TypeConstructor.IsType(entity))
            {
                entity.WrappedType = (IWrappedType)Activator.CreateInstance(_wrapper, entity);
            }
            else
            {
                var resultEntity = entity.Construct(entity.OriginalType);
                entity.WrappedType = (IWrappedType)Activator.CreateInstance(_wrapper, resultEntity);
            }
        }

        class PlainWrappedType : IWrappedType
        {
            public PlainWrappedType(Entity entity)
            {
                Entity = entity;
                Type = entity.TypeConstructor.Derive(entity);
            }

            public Entity Entity { get; set; }

            public GraphType Type { get; set; }

            public object Unwrap(object value)
            {
                return value;
            }

            public object Wrap(object value)
            {
                return value;
            }

            public object FromDictionary(Dictionary<string, object> dictionary)
            {
                throw new NotImplementedException();
            }
        }
    }
}
