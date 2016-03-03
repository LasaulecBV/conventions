using System;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class UnionAttribute : MetaDataAttributeBase
    {
        private readonly Type[] _unionTypes;

        public UnionAttribute(params Type[] unionTypes)
        {
            _unionTypes = unionTypes ?? new Type[0];
        }

        public override void DeriveMetaData(Entity entity)
        {
            var unionEntites = _unionTypes
                .Select(type => entity.Construct(type))
                .Where(unionTypeEntity => !unionTypeEntity.IsIgnored)
                .ToList();

            entity.UnionTypes.AddRange(unionEntites);

            foreach (var unionEntity in unionEntites)
            {
                if (!unionEntity.PossibleTypes.Contains(entity))
                {
                    unionEntity.PossibleTypes.Add(entity);
                }
            }
        }
    }
}
