using System;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Extensions;
using Should;
using Xunit;

namespace Tests.Execution
{
    public class FieldResolution
    {
        [Fact]
        public async Task MinimalSchemaWithNoRoot()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<Data.MinimalQuery>();

            var result = await schema.Execute("{ version date }");
            var expectedResult = "{\"data\":{\"version\":\"1.0\",\"date\":\"0001-01-01T00:00:00Z\"}}";
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public async Task MinimalSchema()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<Data.MinimalQuery>();

            var rootContext = new Data.MinimalQuery { Date = new DateTime(2010, 3, 23, 18, 31, 52) };
            var result = await schema.Execute("{ version date }", rootContext);
            var expectedResult = "{\"data\":{\"version\":\"1.0\",\"date\":\"2010-03-23T18:31:52Z\"}}";
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public async Task ErrorReporting()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<Data.MinimalQuery>();

            var result = await schema.Execute("{ throwException }");
            var expectedResult = "{\"data\":null,\"errors\":[{\"message\":\"Error trying to resolve throwException.\"," +
                "\"locations\":[{\"line\":1,\"column\":2}]}]}";
            result.ShouldEqual(expectedResult);
        }

        class Data
        {
#pragma warning disable 0649

            [Name("Query")]
            public class MinimalQuery
            {
                public string Version = "1.0";

                public DateTime Date { get; set; }

                public bool ThrowException()
                {
                    throw new Exception("This is a test error.");
                }
            }

            #pragma warning restore 0649
        }
    }
}
