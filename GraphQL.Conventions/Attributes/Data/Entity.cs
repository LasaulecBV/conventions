using System;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Attributes.Handlers;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.Data
{
    public class Entity
    {
        private readonly static MetaDataHandler _metaDataHandler = new MetaDataHandler();
        private readonly static MetaDataCollector _metaDataCollector = new MetaDataCollector();
        private readonly static ExecutionFilterCollector _executionFilterCollector = new ExecutionFilterCollector();

        internal Dictionary<Tuple<string, bool>, Entity> Cache { get; private set; }

        internal TypeConstructor TypeConstructor { get; private set; }

        internal Entity()
        {
            Cache = new Dictionary<Tuple<string, bool>, Entity>();
            TypeConstructor = new TypeConstructor();
        }

        public static Entity New(ICustomAttributeProvider obj, bool isInputType = false)
        {
            var entity = new Entity();
            return entity.Construct(obj, isInputType);
        }

        internal Entity Construct(ICustomAttributeProvider obj, bool isInputType = false)
        {
            var entity = new Entity
            {
                Cache = Cache,
                TypeConstructor = TypeConstructor,
            };

            var typeInfo = obj as TypeInfo;
            if (typeInfo != null)
            {
                Entity cachedEntity;
                var key = Tuple.Create(typeInfo.FullName, isInputType);

                if (Cache.TryGetValue(key, out cachedEntity))
                {
                    return cachedEntity;
                }

                Cache.Add(key, entity);
            }

            var metaDataAttributes = _metaDataCollector.CollectAttributes(obj, isInputType);
            _metaDataHandler.DeriveMetaData(entity, metaDataAttributes);

            var executionFilters = _executionFilterCollector.CollectAttributes(obj);
            entity.ExecutionFilters.AddRange(executionFilters);

            return entity;
        }

        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        private Type _typeRepresentation;

        public ICustomAttributeProvider AttributeProvider { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public object DefaultValue { get; set; }

        public Entity DeclaringType { get; set; }

        public Type OriginalType { get; set; }

        public Type TypeRepresentation
        {
            get { return _typeRepresentation ?? OriginalType; }
            set { _typeRepresentation = value; }
        }

        internal IWrappedType WrappedType { get; set; }

        public GraphType GraphType => WrappedType?.Type;

        public List<Entity> Arguments { get; private set; } = new List<Entity>();

        public List<Entity> Fields { get; private set; } = new List<Entity>();

        public string DeprecationReason { get; set; }

        public bool IsDeprecated => !string.IsNullOrWhiteSpace(DeprecationReason);

        public List<Entity> Interfaces { get; private set; } = new List<Entity>();

        public List<Entity> PossibleTypes { get; private set; } = new List<Entity>();

        public List<Entity> UnionTypes { get; private set; } = new List<Entity>();

        public Kind Kind { get; internal set; }

        public bool IsIgnored { get; set; }

        public bool IsInput => Kind == Kind.Argument || Kind == Kind.InputType;

        public bool IsOutput => Kind == Kind.Field || Kind == Kind.OutputType;

        public bool IsNullable { get; set; }

        public bool IsAsynchronous { get; set; }

        public bool IsProfilable { get; set; }

        public Func<ResolveFieldContext, object> Resolver = _ => null;

        internal List<IExecutionFilterAttribute> ExecutionFilters { get; private set; } =
            new List<IExecutionFilterAttribute>();

        public void Set<T>(string key, T value)
        {
            _data[key] = value;
        }

        public T Get<T>(string key)
        {
            object value;
            return _data.TryGetValue(key, out value)
                ? value is T ? (T)value : default(T)
                : default(T);
        }

        public override string ToString()
        {
            return $"Entity({Name}: {TypeRepresentation.Name})";
        }
    }
}
