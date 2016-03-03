using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.Handlers
{
    class MetaDataHandler
    {
        public void DeriveMetaData(Entity entity, IEnumerable<IMetaDataAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.ShouldBeApplied(entity))
                {
                    attribute.DeriveMetaData(entity);
                }
                if (entity.IsIgnored)
                {
                    break;
                }
            }
        }
    }
}
