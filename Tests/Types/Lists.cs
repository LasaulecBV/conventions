using System.Collections.Generic;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Relay;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Types
{
    public class Lists
    {
        [Fact]
        public void NullableString()
        {
            var entity = Entity.New(typeof(List<string>));
            entity.GraphType.ShouldBeType<ListGraphType<StringGraphType>>();

            var input = new List<string> { "first", "second", "third" };

            var wrappedOutput = (List<string>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].ShouldEqual("first");
            wrappedOutput[1].ShouldEqual("second");
            wrappedOutput[2].ShouldEqual("third");

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].ShouldEqual("first");
            unwrappedOutput[1].ShouldEqual("second");
            unwrappedOutput[2].ShouldEqual("third");
        }

        [Fact]
        public void NonNullString()
        {
            var entity = Entity.New(typeof(List<NonNull<string>>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<StringGraphType>>>();

            var input = new List<string> { "first", "second", "third" };

            var wrappedOutput = (List<NonNull<string>>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].Value.ShouldEqual("first");
            wrappedOutput[1].Value.ShouldEqual("second");
            wrappedOutput[2].Value.ShouldEqual("third");

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].ShouldEqual("first");
            unwrappedOutput[1].ShouldEqual("second");
            unwrappedOutput[2].ShouldEqual("third");
        }

        [Fact]
        public void NullablePrimitive()
        {
            var entity = Entity.New(typeof(List<int?>));
            entity.GraphType.ShouldBeType<ListGraphType<IntGraphType>>();

            var input = new List<int?> { 1, null, 99 };

            var wrappedOutput = (List<int?>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].Value.ShouldEqual(1);
            wrappedOutput[1].HasValue.ShouldBeFalse();
            wrappedOutput[2].Value.ShouldEqual(99);

            var unwrappedOutput = (List<int?>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].Value.ShouldEqual(1);
            unwrappedOutput[1].HasValue.ShouldBeFalse();
            unwrappedOutput[2].Value.ShouldEqual(99);
        }

        [Fact]
        public void NonNullPrimitive()
        {
            var entity = Entity.New(typeof(List<int>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<IntGraphType>>>();

            var input = new List<int> { 1, 2, 3 };

            var wrappedOutput = (List<int>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].ShouldEqual(1);
            wrappedOutput[1].ShouldEqual(2);
            wrappedOutput[2].ShouldEqual(3);

            var unwrappedOutput = (List<int>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].ShouldEqual(1);
            unwrappedOutput[1].ShouldEqual(2);
            unwrappedOutput[2].ShouldEqual(3);
        }

        [Fact]
        public void NullableObject()
        {
            var entity = Entity.New(typeof(List<Foo>));
            entity.GraphType.ShouldBeType<ListGraphType<TypeConstructor.ObjectType<Foo>>>();

            var input = new List<Foo>
            {
                new Foo { Data = "first" },
                new Foo { Data = "second" },
                new Foo { Data = "third" },
            };

            var wrappedOutput = (List<Foo>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].Data.ShouldEqual("first");
            wrappedOutput[1].Data.ShouldEqual("second");
            wrappedOutput[2].Data.ShouldEqual("third");

            var unwrappedOutput = (List<Foo>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].Data.ShouldEqual("first");
            unwrappedOutput[1].Data.ShouldEqual("second");
            unwrappedOutput[2].Data.ShouldEqual("third");
        }

        [Fact]
        public void NonNullObject()
        {
            var entity = Entity.New(typeof(List<NonNull<Foo>>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<TypeConstructor.ObjectType<Foo>>>>();

            var input = new List<Foo>
            {
                new Foo { Data = "first" },
                new Foo { Data = "second" },
                new Foo { Data = "third" },
            };

            var wrappedOutput = (List<NonNull<Foo>>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].Value.Data.ShouldEqual("first");
            wrappedOutput[1].Value.Data.ShouldEqual("second");
            wrappedOutput[2].Value.Data.ShouldEqual("third");

            var unwrappedOutput = (List<Foo>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].Data.ShouldEqual("first");
            unwrappedOutput[1].Data.ShouldEqual("second");
            unwrappedOutput[2].Data.ShouldEqual("third");
        }

        [Fact]
        public void NullableEnum()
        {
            var entity = Entity.New(typeof(List<Baz?>));
            entity.GraphType.ShouldBeType<ListGraphType<TypeConstructor.EnumType<Baz>>>();

            var input = new List<Baz?> { Baz.First, null, Baz.Third };

            var wrappedOutput = (List<Baz?>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(3);
            wrappedOutput[0].ShouldEqual(Baz.First);
            wrappedOutput[1].ShouldEqual(null);
            wrappedOutput[2].ShouldEqual(Baz.Third);

            var unwrappedOutput = (List<Baz?>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(3);
            unwrappedOutput[0].ShouldEqual(Baz.First);
            unwrappedOutput[1].ShouldEqual(null);
            unwrappedOutput[2].ShouldEqual(Baz.Third);
        }

        [Fact]
        public void NonNullEnum()
        {
            var entity = Entity.New(typeof(List<Baz>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<TypeConstructor.EnumType<Baz>>>>();

            var input = new List<Baz> { Baz.First, Baz.Third };

            var wrappedOutput = (List<Baz>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(2);
            wrappedOutput[0].ShouldEqual(Baz.First);
            wrappedOutput[1].ShouldEqual(Baz.Third);

            var unwrappedOutput = (List<Baz>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(2);
            unwrappedOutput[0].ShouldEqual(Baz.First);
            unwrappedOutput[1].ShouldEqual(Baz.Third);
        }

        [Fact]
        public void NullableId()
        {
            var entity = Entity.New(typeof(List<Id<Foo>?>));
            entity.GraphType.ShouldBeType<ListGraphType<IdGraphType>>();

            var input = new List<string> { null, (new Id<Foo>("abc")).Value };

            var wrappedOutput = (List<Id<Foo>?>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(2);
            wrappedOutput[0].ShouldEqual(null);
            wrappedOutput[1].ShouldEqual(new Id<Foo>("abc"));

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(2);
            unwrappedOutput[0].ShouldEqual(null);
            unwrappedOutput[1].ShouldEqual((new Id<Foo>("abc")).Value);
        }

        [Fact]
        public void NonNullId()
        {
            var entity = Entity.New(typeof(List<Id<Foo>>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<IdGraphType>>>();

            var input = new List<string> { (new Id<Foo>("def")).Value, (new Id<Foo>("abc")).Value };

            var wrappedOutput = (List<Id<Foo>>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(2);
            wrappedOutput[0].ShouldEqual(new Id<Foo>("def"));
            wrappedOutput[1].ShouldEqual(new Id<Foo>("abc"));

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(2);
            unwrappedOutput[0].ShouldEqual((new Id<Foo>("def")).Value);
            unwrappedOutput[1].ShouldEqual((new Id<Foo>("abc")).Value);
        }

        [Fact]
        public void NullableCursor()
        {
            var entity = Entity.New(typeof(List<Cursor<Foo>?>));
            entity.GraphType.ShouldBeType<ListGraphType<StringGraphType>>();

            var input = new List<string> { null, (new Cursor<Foo>(5)).Value };

            var wrappedOutput = (List<Cursor<Foo>?>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(2);
            wrappedOutput[0].HasValue.ShouldBeFalse();
            wrappedOutput[1].ShouldEqual(new Cursor<Foo>(5));

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(2);
            unwrappedOutput[0].ShouldEqual(null);
            unwrappedOutput[1].ShouldEqual((new Cursor<Foo>(5)).Value);
        }

        [Fact]
        public void NonNullCursor()
        {
            var entity = Entity.New(typeof(List<Cursor<Foo>>));
            entity.GraphType.ShouldBeType<ListGraphType<NonNullGraphType<StringGraphType>>>();

            var input = new List<string> { (new Cursor<Foo>(99)).Value, (new Cursor<Foo>(105)).Value };

            var wrappedOutput = (List<Cursor<Foo>>)entity.WrappedType.Wrap(input);
            wrappedOutput.Count.ShouldEqual(2);
            wrappedOutput[0].ShouldEqual(new Cursor<Foo>(99));
            wrappedOutput[1].ShouldEqual(new Cursor<Foo>(105));

            var unwrappedOutput = (List<string>)entity.WrappedType.Unwrap(wrappedOutput);
            unwrappedOutput.Count.ShouldEqual(2);
            unwrappedOutput[0].ShouldEqual((new Cursor<Foo>(99)).Value);
            unwrappedOutput[1].ShouldEqual((new Cursor<Foo>(105)).Value);
        }

        class Foo
        {
            public string Data;
        }

        enum Baz
        {
            First,
            Second,
            Third
        }
    }
}
