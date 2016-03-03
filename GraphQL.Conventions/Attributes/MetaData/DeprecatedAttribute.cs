using System;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class DeprecatedAttribute : MetaDataAttributeBase
    {
        private readonly string _deprecationReason;

        public DeprecatedAttribute(string deprecationReason)
        {
            _deprecationReason = deprecationReason;
        }

        public override void DeriveMetaData(Entity entity)
        {
            entity.DeprecationReason = _deprecationReason;
        }
    }
}
