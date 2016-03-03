using System;
using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Profiling;
using GraphQL.Types;

namespace GraphQL.Conventions
{
    public class Schema : GraphQL.Types.Schema
    {
        private readonly Entity _entityConstructor = new Entity();
        private readonly List<IProfiler> _profilers = new List<IProfiler>();

        private Schema() { }

        public static Schema New()
        {
            return new Schema();
        }

        public Schema WithQuery<TQuery>()
        {
            return WithQuery(typeof(TQuery));
        }

        public Schema WithQuery(Type queryType)
        {
            var queryEntity = _entityConstructor.Construct(queryType);
            Query = queryEntity.GraphType as ObjectGraphType;
            return this;
        }

        public Schema WithMutation<TMutation>()
        {
            return WithMutation(typeof(TMutation));
        }

        public Schema WithMutation(Type mutationType)
        {
            var mutationEntity = _entityConstructor.Construct(mutationType);
            Mutation = mutationEntity.GraphType as ObjectGraphType;
            return this;
        }

        public Schema UseProfilers(params IProfiler[] profilers)
        {
            _profilers.AddRange(profilers);
            return this;
        }

        public Schema WithTypeResolver(Func<Type, object> resolver)
        {
            _entityConstructor.TypeConstructor.TypeResolver = resolver;
            return this;
        }
    }
}
