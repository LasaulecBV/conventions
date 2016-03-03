using System.Linq;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests
{
    public class Schema
    {
        [Fact]
        public void WithQuery()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<TestQuery>();

            schema.Query.Description.ShouldEqual("A couple of queries");
            schema.Query.Fields.Count().ShouldEqual(2);
            schema.Query.ShouldHaveField("foo");
            schema.Query.ShouldHaveField("bar");
            schema.Mutation.ShouldBeNull();
        }

        [Fact]
        public void WithQueryAndMutation()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<TestQuery>()
                .WithMutation<TestMutation>();

            schema.Query.Fields.Count().ShouldEqual(2);
            schema.Query.ShouldHaveField("foo");
            schema.Query.ShouldHaveField("bar");

            schema.Mutation.Fields.Count().ShouldEqual(1);
            schema.Mutation.ShouldHaveField("setFoo");
        }

        [Description("A couple of queries")]
        class TestQuery
        {
            public string Foo => "12345";

            public bool Bar => true;
        }

        class TestMutation
        {
            public bool SetFoo(string value)
            {
                return value != null;
            }
        }
    }
}
