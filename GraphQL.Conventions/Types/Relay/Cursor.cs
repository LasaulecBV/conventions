using System;
using System.Globalization;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;

namespace GraphQL.Conventions.Types.Relay
{
    [Type(typeof(string))]
    [Description("Cursor used in pagination.")]
    [Converter(typeof(Cursor))]
    public struct Cursor<T> : IComparable, IComparable<Cursor<T>>, IEquatable<Cursor<T>>
    {
        private readonly Id<T> _cursor;

        private Cursor(string id)
        {
            _cursor = new Id<T>(id);
            Index = GetValue(id);
        }

        public Cursor(long cursor)
            : this(Math.Max(0, cursor).ToString())
        { }

        internal static Cursor<T> FromFirstNonNullString(params string[] cursors)
        {
            foreach (var cursor in cursors)
            {
                if (!string.IsNullOrWhiteSpace(cursor))
                {
                    return FromString(cursor);
                }
            }
            throw new ArgumentNullException(nameof(cursors));
        }

        public static Cursor<T> FromString(string cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return new Cursor<T>();
            }

            var underlyingId = Id<T>.FromString(cursor).UnderlyingIdentifier;
            var value = GetValue(underlyingId);
            return new Cursor<T>(value);
        }

        public string Value => _cursor.Value;

        public long Index { get; }

        public static implicit operator Cursor<T>(string cursor)
        {
            return FromString(cursor);
        }

        public static implicit operator Cursor<T>?(string cursor)
        {
            return string.IsNullOrWhiteSpace(cursor)
                ? (Cursor<T>?)null
                : FromString(cursor);
        }

        public static implicit operator Cursor<T>(long cursor)
        {
            return new Cursor<T>(cursor);
        }

        public static implicit operator Cursor<T>(DateTime cursor)
        {
            return new Cursor<T>(-cursor.Ticks);
        }

        public static implicit operator string(Cursor<T> id)
        {
            return id.Value;
        }

        public static implicit operator string(Cursor<T>? id)
        {
            return id?.Value;
        }

        public static implicit operator long(Cursor<T> id)
        {
            return id.Index;
        }

        public static implicit operator DateTime(Cursor<T> id)
        {
            var ticks = -id.Index;
            return new DateTime(ticks);
        }

        public static bool operator ==(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return cursor1.Equals(cursor2);
        }

        public static bool operator !=(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return !cursor1.Equals(cursor2);
        }

        public static bool operator <(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return cursor1.CompareTo(cursor2) < 0;
        }

        public static bool operator >(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return cursor1.CompareTo(cursor2) > 0;
        }

        public static bool operator <=(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return cursor1.CompareTo(cursor2) <= 0;
        }

        public static bool operator >=(Cursor<T> cursor1, Cursor<T> cursor2)
        {
            return cursor1.CompareTo(cursor2) >= 0;
        }

        public override bool Equals(object other)
        {
            if (other is Cursor<T>)
            {
                var otherCursor = (Cursor<T>)other;
                return _cursor == otherCursor._cursor;
            }
            return false;
        }

        public bool Equals(Cursor<T> other)
        {
            return _cursor == other._cursor;
        }

        public int CompareTo(object other)
        {
            if (other is Cursor<T>)
            {
                var otherCursor = (Cursor<T>)other;
                return _cursor.CompareTo(otherCursor._cursor);
            }
            return -1;
        }

        public int CompareTo(Cursor<T> other)
        {
            return Index.CompareTo(other.Index);
        }

        public override int GetHashCode()
        {
            return _cursor.GetHashCode();
        }

        public override string ToString()
        {
            return _cursor.Value;
        }

        private static int GetValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Cannot construct cursor from null value.");
            }

            int index;

            if (!string.IsNullOrWhiteSpace(value) &&
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out index))
            {
                return index;
            }

            throw new ArgumentNullException(nameof(value), $"Unable to construct cursor from value '{value}'.");
        }
    }

    internal class Cursor : WrappedType
    {
        public Cursor(Entity entity)
            : base(entity)
        {
            Type = new NonNullGraphType<StringGraphType>();
        }
    }
}
