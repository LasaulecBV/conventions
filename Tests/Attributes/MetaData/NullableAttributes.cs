using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class NullableAttributes
    {
        #pragma warning disable 0649

        public class Types
        {
            [Fact]
            public void NonNullable()
            {
                var typeInfo = typeof(Data.NonNullableClass);
                var entity = Entity.New(typeInfo);
                entity.IsNullable.ShouldBeFalse();
            }

            [Fact]
            public void Nullable()
            {
                var typeInfo = typeof(Data.NullableClass);
                var entity = Entity.New(typeInfo);
                entity.IsNullable.ShouldBeTrue();
            }

            class Data
            {
                [NonNullable]
                public class NonNullableClass { }

                [Nullable]
                public class NullableClass { }
            }
        }

        public class Interfaces
        {
            [Fact]
            public void NonNullable()
            {
                var typeInfo = typeof(Data.NonNullableInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.First().IsNullable.ShouldBeFalse();
            }

            [Fact]
            public void Nullable()
            {
                var typeInfo = typeof(Data.NullableInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.First().IsNullable.ShouldBeTrue();
            }

            class Data
            {
                public class NonNullableInterface : INonNullable { }

                public class NullableInterface : INullable { }

                [NonNullable]
                public interface INonNullable { }

                [Nullable]
                public interface INullable { }
            }
        }

        public class Unions
        {
            [Fact]
            public void NonNullable()
            {
                var typeInfo = typeof(Data.UnionWithNonNullableType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.First().IsNullable.ShouldBeFalse();
            }

            [Fact]
            public void Nullable()
            {
                var typeInfo = typeof(Data.UnionWithNullableType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.First().IsNullable.ShouldBeTrue();
            }

            class Data
            {
                [Union(typeof(NonNullableType))]
                public class UnionWithNonNullableType { }

                [Union(typeof(NullableType))]
                public class UnionWithNullableType { }

                [NonNullable]
                public class NonNullableType { }

                [Nullable]
                public class NullableType { }
            }
        }

        #pragma warning restore 0649
    }
}
