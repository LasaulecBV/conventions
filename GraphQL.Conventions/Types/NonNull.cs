using System;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Types
{
    [Description("Non-nullable object.")]
    [NonNullable, Type]
    [Converter(typeof(NonNull))]
    public struct NonNull<T> : IEquatable<NonNull<T>>
        where T : class
    {
        private readonly T _value;

        public NonNull(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = value;
        }

        public T Value => _value;

        public static implicit operator NonNull<T>(T value)
        {
            return new NonNull<T>(value);
        }

        public static implicit operator T(NonNull<T> value)
        {
            return value._value;
        }

        public static bool operator ==(NonNull<T> v1, NonNull<T> v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(NonNull<T> v1, NonNull<T> v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object other)
        {
            if (other is NonNull<T>)
            {
                var otherValue = (NonNull<T>)other;
                return _value == otherValue._value;
            }
            return false;
        }

        public bool Equals(NonNull<T> other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    internal class NonNull : WrappedType
    {
        public NonNull(Entity entity)
            : base(entity)
        {
            if (TypeConstructor.IsType(entity) && entity.WrappedType != null)
            {
                var baseTypeEntity = entity.Construct(entity.TypeRepresentation);
                var baseType = baseTypeEntity.GraphType.GetType();
                var type = IsNonNullGraphType(baseType) ? baseType : typeof(NonNullGraphType<>).MakeGenericType(baseType);
                Type = (GraphType)Activator.CreateInstance(type);
            }
        }

        private static bool IsNonNullGraphType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>);
        }
    }
}
