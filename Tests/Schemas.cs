using Should;
using Xunit;

namespace Tests
{
    public class Schemas
    {
        [Fact]
        public void CombineEmptySchemas()
        {
            var schema = GraphQL.Conventions.Schemas
                .New()
                .With(new Schema1())
                .With(new Schema2())
                .Combine();

            schema.Query.ShouldBeNull();
            schema.Mutation.ShouldBeNull();
        }

        class Schema1 : GraphQL.Types.Schema
        {
        }

        class Schema2  : GraphQL.Types.Schema
        {
        }
    }
}
