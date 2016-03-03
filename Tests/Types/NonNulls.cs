using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Types
{
    public class NonNulls
    {
        [Fact]
        public void IsOutputObject()
        {
            var entity = Entity.New(typeof(NonNull<Foo>));

            entity.Name.ShouldEqual("Foo");
            entity.Description.ShouldEqual("Foo bar baz.");
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(Foo));
            entity.OriginalType.ShouldEqual(typeof(NonNull<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeFalse();

            entity.Fields.Count.ShouldEqual(1);
            entity.ShouldHaveField("someField");

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        [Fact]
        public void IsInputObject()
        {
            var entity = Entity.New(typeof(NonNull<Foo>), true);

            entity.Name.ShouldEqual("FooInput");
            entity.Description.ShouldEqual("Foo bar baz.");
            entity.Kind.ShouldEqual(Kind.InputType);
            entity.TypeRepresentation.ShouldEqual(typeof(Foo));
            entity.OriginalType.ShouldEqual(typeof(NonNull<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeFalse();
            entity.IsInput.ShouldBeTrue();
            entity.IsNullable.ShouldBeFalse();

            entity.Fields.Count.ShouldEqual(1);
            entity.ShouldHaveField("someField");

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        [Fact]
        public void InstantiatesFromString()
        {
            var entity = Entity.New(typeof(NonNull<string>));
            entity.GraphType.ShouldBeType<NonNullGraphType<StringGraphType>>();

            var input = "blah";
            var output = (NonNull<string>)entity.WrappedType.Wrap(input);
            output.Value.ShouldEqual("blah");
        }

        [Fact]
        public void ReturnsString()
        {
            var entity = Entity.New(typeof(NonNull<string>));
            entity.GraphType.ShouldBeType<NonNullGraphType<StringGraphType>>();

            var input = new NonNull<string>("blah");
            var output = entity.WrappedType.Unwrap(input);
            output.ShouldEqual("blah");
        }


        [Fact]
        public void InstantiatesFromObject()
        {
            var entity = Entity.New(typeof(NonNull<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<TypeConstructor.ObjectType<Foo>>>();

            var input = new Foo { SomeField = true };
            var output = (NonNull<Foo>)entity.WrappedType.Wrap(input);
            output.Value.SomeField.ShouldEqual(true);
        }

        [Fact]
        public void ReturnsObject()
        {
            var entity = Entity.New(typeof(NonNull<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<TypeConstructor.ObjectType<Foo>>>();

            var input = new NonNull<Foo>(new Foo { SomeField = true });
            var output = (Foo)entity.WrappedType.Unwrap(input);
            output.SomeField.ShouldEqual(true);
        }

        [Description("Foo bar baz.")]
        private class Foo
        {
            public bool SomeField { get; set; }
        }
    }
}
