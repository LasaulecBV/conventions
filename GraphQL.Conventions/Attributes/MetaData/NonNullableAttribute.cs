using System;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Types;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(
        AttributeTargets.Class|AttributeTargets.Struct|AttributeTargets.Interface,
        AllowMultiple = true,
        Inherited = true)]
    class NonNullableAttribute : MetaDataAttributeBase
    {
        private readonly bool _detectValueTypes;

        public NonNullableAttribute()
        {
            Order = TypeAttribute.InterjectionOrder;
            AssociatedAttributes.Add(new MetaDataAttribute());
        }

        internal NonNullableAttribute(bool detectValueTypes)
        {
            _detectValueTypes = detectValueTypes;
            Order = ConverterAttribute.OverrideOrder;
            AssociatedAttributes.Add(new NonNullConverterAttribute());
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (!_detectValueTypes)
            {
                entity.IsNullable = false;
            }

            if (entity.TypeRepresentation.IsGenericType &&
                entity.TypeRepresentation.GetGenericTypeDefinition() == typeof(NonNull<>))
            {
                entity.IsNullable = false;
                entity.TypeRepresentation = entity.TypeRepresentation.GenericTypeArguments.First();
            }
            else if (_detectValueTypes && entity.TypeRepresentation.IsValueType)
            {
                entity.IsNullable = false;
            }
        }

        private class MetaDataAttribute : MetaDataAttributeBase
        {
            public MetaDataAttribute()
            {
                Order = DescriptionAttribute.OverrideOrder;
            }

            public override void DeriveMetaData(Entity entity)
            {
                if (entity.OriginalType.IsGenericType &&
                    entity.OriginalType.GetGenericTypeDefinition() == typeof(NonNull<>))
                {
                    entity.Name = entity.Name.Replace(nameof(NonNull), string.Empty);

                    var description = entity.Construct(entity.TypeRepresentation).Description;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        entity.Description = description;
                    }
                }
            }
        }

        private class NonNullConverterAttribute : ConverterAttribute
        {
            public NonNullConverterAttribute()
                : base(typeof(NonNull))
            {
            }

            public override void DeriveMetaData(Entity entity)
            {
                if (entity.TypeRepresentation.IsValueType)
                {
                    base.DeriveMetaData(entity);
                }
            }
        }
    }
}
