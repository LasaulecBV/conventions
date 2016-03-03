using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class AsyncAttributes
    {
        #pragma warning disable 0649

        [Fact]
        public void Primitive()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.Primitive));
            var entity = Entity.New(methodInfo);
            entity.TypeRepresentation.ShouldEqual(typeof(int));
            entity.GraphType.ShouldBeType<NonNullGraphType<IntGraphType>>();
            entity.Kind.ShouldEqual(Kind.Field);
            entity.IsAsynchronous.ShouldBeTrue();

            throw new NotImplementedException("XXXPageInfo => PageInfo");
            throw new NotImplementedException("StringList etc => [String]"); // Fixed? Not quite : [[MessageContentList]]
            // TODO tidy up OriginalType vs TypeRepresentation, DeclaringType, etc. (type derivation in general) - and consequently also unit tests
            throw new NotImplementedException("Type instance to type sometimes yield wrong type (e.g., UserList, TeamList, ... all yield UserList, etc.)");
            throw new NotImplementedException("Global / shared entity => no domain-specific caching (in ObjectType<>, et al)");
        }

        [Fact]
        public void Object()
        {
            false.ShouldBeTrue();
        }

        [Fact]
        public void NonNull()
        {
            false.ShouldBeTrue();
        }


        [Fact]
        public void Nullable()
        {
            false.ShouldBeTrue();
        }

        [Fact]
        public void List()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.List));
            var entity = Entity.New(methodInfo);
            entity.TypeRepresentation.ShouldEqual(typeof(List<int>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<IntGraphType>>>();
            entity.Kind.ShouldEqual(Kind.Field);
            entity.IsAsynchronous.ShouldBeTrue();
        }

        [Fact]
        public void Id()
        {
            false.ShouldBeTrue();
        }

        [Fact]
        public void Cursor()
        {
            false.ShouldBeTrue();
        }

        [Fact]
        public void Enum()
        {
            false.ShouldBeTrue();
        }

        class Foo
        {
            public Task<int> Primitive() { return Task.FromResult(0); }

            public Task<List<int>> List() { return Task.FromResult(new List<int>()); }
        }

        #pragma warning restore 0649
    }
}
