using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Relay;
using Should;
using Xunit;

namespace Tests.Types.Relay
{
    public class Edges
    {
        [Fact]
        public void IsOutputObject()
        {
            var entity = Entity.New(typeof(Edge<Foo>));

            entity.Name.ShouldEqual("FooEdge");
            entity.Description.ShouldNotBeEmpty();
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(Edge<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeTrue();

            entity.Fields.Count.ShouldEqual(2);
            entity.ShouldHaveField("cursor");
            entity.ShouldHaveField("node");

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        private class Foo { }
    }
}
