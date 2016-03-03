using System;
using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Attributes.Interfaces
{
    public abstract class MetaDataAttributeBase : Attribute, IMetaDataAttribute
    {
        public List<IMetaDataAttribute> AssociatedAttributes { get; private set; } = new List<IMetaDataAttribute>();

        public int Order { get; set; }

        public virtual bool ShouldBeApplied(Entity entity)
        {
            return true;
        }

        public abstract void DeriveMetaData(Entity entity);
    }
}
