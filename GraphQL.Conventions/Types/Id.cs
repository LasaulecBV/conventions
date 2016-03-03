using System;
using System.Text;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Types
{
    [Type(typeof(string))]
    [Description("Globally unique identifier.")]
    [Converter(typeof(Id))]
    public struct Id<T> : IComparable, IComparable<Id<T>>, IEquatable<Id<T>>
    {
        private readonly string _identifier;

        public Id(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(
                    nameof(identifier),
                    $"Unable to construct an identifier of type Id<{typeof(T).Name}> from an empty string.");
            }

            UnderlyingIdentifier = identifier;
            _identifier = Encode(identifier);
        }

        internal static Id<T> FromString(string id)
        {
            var decodedIdentifier = Decode(id);
            if (string.IsNullOrWhiteSpace(decodedIdentifier))
            {
                throw new ArgumentException(
                    $"Unable to construct an identifier of type Id<{typeof(T).Name}> from value '{id}'.",
                    nameof(id));
            }
            return new Id<T>(decodedIdentifier);
        }

        public string Value => _identifier;

        public string UnderlyingIdentifier { get; }

        public static implicit operator Id<T>(string identifier)
        {
            return FromString(identifier);
        }

        public static implicit operator Id<T>?(string identifier)
        {
            return string.IsNullOrWhiteSpace(identifier)
                ? (Id<T>?)null
                : FromString(identifier);
        }

        public static implicit operator string(Id<T> id)
        {
            return id.Value;
        }

        public static implicit operator string(Id<T>? id)
        {
            return id?.Value;
        }

        public static bool operator ==(Id<T> id1, Id<T> id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(Id<T> id1, Id<T> id2)
        {
            return !id1.Equals(id2);
        }

        public static bool operator <(Id<T> id1, Id<T> id2)
        {
            return id1.CompareTo(id2) == -1;
        }

        public static bool operator >(Id<T> id1, Id<T> id2)
        {
            return id1.CompareTo(id2) == 1;
        }

        public static bool operator <=(Id<T> id1, Id<T> id2)
        {
            return id1.CompareTo(id2) <= 0;
        }

        public static bool operator >=(Id<T> id1, Id<T> id2)
        {
            return id1.CompareTo(id2) >= 0;
        }

        public override bool Equals(object other)
        {
            if (other is Id<T>)
            {
                var otherId = (Id<T>)other;
                return _identifier == otherId._identifier;
            }
            return false;
        }

        public bool Equals(Id<T> other)
        {
            return _identifier == other._identifier;
        }

        public int CompareTo(object other)
        {
            if (other is Id<T>)
            {
                var otherId = (Id<T>)other;
                return string.Compare(UnderlyingIdentifier, otherId.UnderlyingIdentifier, StringComparison.Ordinal);
            }
            return -1;
        }

        public int CompareTo(Id<T> other)
        {
            return string.Compare(UnderlyingIdentifier, other.UnderlyingIdentifier, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }

        public override string ToString()
        {
            return _identifier;
        }

        private static string Encode(string id)
        {
            var encodedId = $"{typeof(T).Name}{id}";
            var bytes = Encoding.UTF8.GetBytes(encodedId);
            return Convert.ToBase64String(bytes);
        }

        private static string Decode(string encodedId)
        {
            if (encodedId == null)
            {
                return null;
            }

            try
            {
                var bytes = Convert.FromBase64String(encodedId);
                var decodedIdentifier = Encoding.UTF8.GetString(bytes);

                return !decodedIdentifier.StartsWith(typeof(T).Name)
                    ? null
                    : decodedIdentifier.Remove(0, typeof(T).Name.Length);
            }
            catch
            {
                return null;
            }
        }
    }

    internal class Id : WrappedType
    {
        public Id(Entity entity)
            : base(entity)
        {
            Type = new NonNullGraphType<IdGraphType>();
        }
    }
}
