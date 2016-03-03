using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Types
{
    public class Ids
    {
        [Fact]
        public void IsValueType()
        {
            var entity = Entity.New(typeof(Id<Foo>));

            entity.Name.ShouldEqual("FooId");
            entity.Description.ShouldEqual("Globally unique identifier.");
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(string));
            entity.OriginalType.ShouldEqual(typeof(Id<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeFalse();

            entity.Fields.Count.ShouldEqual(0);

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        [Fact]
        public void InstantiatesFromString()
        {
            var entity = Entity.New(typeof(Id<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<IdGraphType>>();

            var input = "Rm9vMTIzNDU=";
            var output = (Id<Foo>)entity.WrappedType.Wrap(input);
            output.UnderlyingIdentifier.ShouldEqual("12345");
        }

        [Fact]
        public void ReturnsString()
        {
            var entity = Entity.New(typeof(Id<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<IdGraphType>>();

            var input = new Id<Foo>("12345");
            var output = entity.WrappedType.Unwrap(input);
            output.ShouldEqual("Rm9vMTIzNDU=");
        }

        private class Foo { }
    }
}
