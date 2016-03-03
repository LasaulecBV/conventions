using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(
        AttributeTargets.Class|AttributeTargets.Struct|AttributeTargets.Interface,
        AllowMultiple = true,
        Inherited = true)]
    class NullableAttribute : ConverterAttribute
    {
        public NullableAttribute()
            : base(typeof(Types.Nullable))
        {
        }

        public override bool ShouldBeApplied(Entity entity)
        {
            var typeInfo = entity.AttributeProvider as TypeInfo;
            if (typeInfo != null)
            {
                return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
            }
            return false;
        }

        public override void DeriveMetaData(Entity entity)
        {
            entity.IsNullable = true;
            base.DeriveMetaData(entity);
        }
    }
}
