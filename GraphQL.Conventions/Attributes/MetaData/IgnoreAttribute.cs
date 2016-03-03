using System;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class IgnoreAttribute : MetaDataAttributeBase
    {
        public const int DefaultOrder = int.MinValue;

        public IgnoreAttribute()
        {
            Order = DefaultOrder;
        }

        public override void DeriveMetaData(Entity entity)
        {
            entity.IsIgnored = true;
        }
    }
}
