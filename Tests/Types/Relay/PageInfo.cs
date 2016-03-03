using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Relay;
using Should;
using Xunit;

namespace Tests.Types.Relay
{
    public class PageInfo
    {
        [Fact]
        public void IsOutputObject()
        {
            var entity = Entity.New(typeof(PageInfo<Foo>));

            entity.Name.ShouldEqual("FooPageInfo");
            entity.Description.ShouldNotBeEmpty();
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(PageInfo<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeTrue();

            entity.Fields.Count.ShouldEqual(4);
            entity.ShouldHaveField("startCursor");
            entity.ShouldHaveField("endCursor");
            entity.ShouldHaveField("hasNextPage");
            entity.ShouldHaveField("hasPreviousPage");

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        private class Foo { }
    }
}
