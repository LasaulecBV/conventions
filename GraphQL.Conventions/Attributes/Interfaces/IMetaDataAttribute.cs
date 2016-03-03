using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Attributes.Interfaces
{
    public interface IMetaDataAttribute : IAttribute
    {
        List<IMetaDataAttribute> AssociatedAttributes { get; }

        int Order { get; }

        bool ShouldBeApplied(Entity entity);

        void DeriveMetaData(Entity entity);
    }
}
