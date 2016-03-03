using System;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Types
{
    public class Nullables
    {
        [Fact]
        public void Primitive()
        {
            var entity = Entity.New(typeof(int));
            entity.GraphType.ShouldBeType<NonNullGraphType<IntGraphType>>();

            Assert.Throws<NullReferenceException>(() => ((int)entity.WrappedType.Wrap(null)));
            ((int)entity.WrappedType.Wrap(0)).ShouldEqual(0);
            ((int)entity.WrappedType.Wrap(1)).ShouldEqual(1);
            ((int)entity.WrappedType.Wrap(99)).ShouldEqual(99);

            Assert.Throws<NullReferenceException>(() => ((int)entity.WrappedType.Unwrap(null)));
            ((int)entity.WrappedType.Unwrap(0)).ShouldEqual(0);
            ((int)entity.WrappedType.Unwrap(1)).ShouldEqual(1);
            ((int)entity.WrappedType.Unwrap(99)).ShouldEqual(99);
        }

        [Fact]
        public void NonNull()
        {
            var entity = Entity.New(typeof(NonNull<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<TypeConstructor.ObjectType<Foo>>>();

            Assert.Throws<ArgumentNullException>(() => ((NonNull<Foo>)entity.WrappedType.Wrap(null)));
            ((NonNull<Foo>)entity.WrappedType.Wrap(new Foo { Data = "1234" })).Value.Data.ShouldEqual("1234");

            Assert.Throws<ArgumentNullException>(() => ((Foo)entity.WrappedType.Unwrap((NonNull<Foo>)null)));
            ((Foo)entity.WrappedType.Unwrap(new NonNull<Foo>(new Foo { Data = "1234" }))).Data.ShouldEqual("1234");
        }

        [Fact]
        public void Object()
        {
            var entity = Entity.New(typeof(Foo));
            entity.GraphType.ShouldBeType<TypeConstructor.ObjectType<Foo>>();
            
            ((Foo)entity.WrappedType.Wrap(null)).ShouldEqual(null);
            ((Foo)entity.WrappedType.Wrap(new Foo { Data = "1234" })).Data.ShouldEqual("1234");

            ((Foo)entity.WrappedType.Unwrap(null)).ShouldEqual(null);
            ((Foo)entity.WrappedType.Unwrap(new Foo { Data = "1234" })).Data.ShouldEqual("1234");
        }

        [Fact]
        public void Enum()
        {
            var entity = Entity.New(typeof(Baz?));
            entity.GraphType.ShouldBeType<TypeConstructor.EnumType<Baz>>();

            ((Baz?)entity.WrappedType.Wrap(Baz.First)).ShouldEqual(Baz.First);
            ((Baz?)entity.WrappedType.Wrap(null)).ShouldEqual(null);
            ((Baz?)entity.WrappedType.Wrap(Baz.Third)).ShouldEqual(Baz.Third);

            ((Baz?)entity.WrappedType.Unwrap(Baz.First)).ShouldEqual(Baz.First);
            ((Baz?)entity.WrappedType.Unwrap(null)).ShouldEqual(null);
            ((Baz?)entity.WrappedType.Unwrap(Baz.Third)).ShouldEqual(Baz.Third);
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
