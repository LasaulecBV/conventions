using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Types;

namespace GraphQL.Conventions.Reflection
{
    class TypeConstructor
    {
        private readonly static Dictionary<Type, GraphType> _primitives = new Dictionary<Type, GraphType>
        {
            { typeof(void), new BooleanGraphType() },
            { typeof(string), new StringGraphType() },
            { typeof(bool), new BooleanGraphType() },
            { typeof(byte), new IntGraphType() },
            { typeof(char), new IntGraphType() },
            { typeof(short), new IntGraphType() },
            { typeof(ushort), new IntGraphType() },
            { typeof(int), new IntGraphType() },
            { typeof(uint), new IntGraphType() },
            { typeof(long), new IntGraphType() },
            { typeof(ulong), new IntGraphType() },
            { typeof(float), new FloatGraphType() },
            { typeof(double), new FloatGraphType() },
            { typeof(decimal), new DecimalGraphType() },
            { typeof(DateTime), new DateGraphType() },
        };

        private readonly Dictionary<Tuple<string, bool>, GraphType> _cache =
            new Dictionary<Tuple<string, bool>, GraphType>();

        public static bool IsType(Entity entity)
        {
            return entity.Kind == Kind.OutputType ||
                   entity.Kind == Kind.InputType ||
                   entity.Kind == Kind.EnumType ||
                   entity.Kind == Kind.InterfaceType ||
                   entity.Kind == Kind.UnionType;
        }

        public object CreateInstance(Type type)
        {
            return (TypeResolver != null)
                ?  TypeResolver(type)
                : Activator.CreateInstance(type);
        }

        public Func<Type, object> TypeResolver { get; set; }

        public GraphType Derive(Entity entity)
        {
            if (!IsType(entity))
            {
                return null;
            }

            GraphType cachedType;
            var key = Tuple.Create(entity.Name, entity.IsInput);

            if (_cache.TryGetValue(key, out cachedType))
            {
                return cachedType;
            }

            GraphType graphType;
            var entityType = entity.TypeRepresentation;

            if (_primitives.TryGetValue(entityType, out graphType))
            {
                return graphType;
            }

            switch (entity.Kind)
            {
                case Kind.OutputType:
                    var outputType = (ObjectGraphType)Activator.CreateInstance(
                        typeof(ObjectType<>).MakeGenericType(entityType));
                    graphType = outputType;
                    _cache.Add(key, graphType);
                    break;
                case Kind.InputType:
                    var inputType = (InputObjectGraphType)Activator.CreateInstance(
                        typeof(InputObjectType<>).MakeGenericType(entityType));
                    graphType = inputType;
                    _cache.Add(key, graphType);
                    break;
                case Kind.InterfaceType:
                    var interfaceType = (InterfaceGraphType)Activator.CreateInstance(
                        typeof(InterfaceType<>).MakeGenericType(entityType));
                    graphType = interfaceType;
                    _cache.Add(key, graphType);
                    break;
                case Kind.UnionType:
                    var unionType = (UnionGraphType)Activator.CreateInstance(
                        typeof(UnionType<>).MakeGenericType(entityType));
                    graphType = unionType;
                    _cache.Add(key, graphType);
                    break;
                case Kind.EnumType:
                    var enumType = (EnumerationGraphType)Activator.CreateInstance(
                        typeof(EnumType<>).MakeGenericType(entityType));
                    graphType = enumType;
                    _cache.Add(key, graphType);
                    break;
            }

            return graphType;
        }

        private static void AddFields(GraphType type, Entity entity)
        {
            foreach (var field in entity.Fields)
            {
                var args = field.Arguments
                    .Select(arg => new QueryArgument(arg.GraphType.GetType())
                    {
                        Name = arg.Name,
                        Description = arg.Description,
                        DefaultValue = arg.DefaultValue,
                    });

                var fieldDefinition = type.Field(
                    field.GraphType.GetType(),
                    field.Name,
                    field.Description,
                    new QueryArguments(args),
                    field.Resolver);

                fieldDefinition.DeprecationReason = field.DeprecationReason;
            }
        }

        private static Entity _entity = new Entity();

        internal class ObjectType<T> : ObjectGraphType
        {
            public ObjectType()
            {
                var entity = _entity.Construct(typeof(T));
                Name = entity.Name;
                Description = entity.Description;
                foreach (var iface in entity.Interfaces)
                {
                    Interface(entity.TypeConstructor.Derive(iface).GetType());
                }
                AddFields(this, entity);
            }
        }

        internal class InputObjectType<T> : InputObjectGraphType
        {
            public InputObjectType()
            {
                var entity = _entity.Construct(typeof(T), true);
                Name = entity.Name;
                Description = entity.Description;
                AddFields(this, entity);
            }
        }

        internal class InterfaceType<T> : InterfaceGraphType
        {
            public InterfaceType()
            {
                var entity = _entity.Construct(typeof(T));
                Name = entity.Name;
                Description = entity.Description;
                foreach (var possibleType in entity.PossibleTypes)
                {
                    var objectType = entity.TypeConstructor.Derive(possibleType) as ObjectGraphType;
                    if (objectType != null)
                    {
                        AddPossibleType(objectType);
                    }
                }
                AddFields(this, entity);
            }
        }

        internal class UnionType<T> : UnionGraphType
        {
            public UnionType()
            {
                var entity = _entity.Construct(typeof(T));
                Name = entity.Name;
                Description = entity.Description;
                foreach (var possibleType in entity.UnionTypes)
                {
                    var objectType = entity.TypeConstructor.Derive(possibleType) as ObjectGraphType;
                    if (objectType != null)
                    {
                        AddPossibleType(objectType);
                    }
                }
                AddFields(this, entity);
            }
        }

        internal class EnumType<T> : EnumerationGraphType
        {
            public EnumType()
            {
                var entity = _entity.Construct(typeof(T));
                Name = entity.Name;
                Description = entity.Description;
                foreach (var value in entity.Fields)
                {
                    AddValue(value.Name, value.Description, value.DefaultValue, value.DeprecationReason);
                }
            }
        }
    }
}
