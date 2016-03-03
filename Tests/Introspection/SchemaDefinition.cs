using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Extensions;
using Should;
using Xunit;

namespace Tests.Introspection
{
    public class SchemaDefinition
    {
        [Fact]
        public void MinimalSchema()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<Data.MinimalQuery>();

            var expectedDefinition = @"""
                | type Query {
                |   version: String
                | }
                |""".Clean();

            schema.GetSchemaDefinition().ShouldEqual(expectedDefinition);
        }

        class Data
        {
            #pragma warning disable 0649

            [Name("Query")]
            public class MinimalQuery
            {
                public string Version;
            }

            #pragma warning restore 0649
        }
    }
}
