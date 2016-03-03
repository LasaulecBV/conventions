using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types;

namespace GraphQL.Conventions.Attributes.MetaData
{
    class ListAttribute : ConverterAttribute
    {
        public ListAttribute()
            : base(typeof(List))
        {
        }

        public override bool ShouldBeApplied(Entity entity)
        {
            return entity.OriginalType.IsGenericType &&
                   entity.OriginalType.IsGenericType &&
                   entity.OriginalType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
