using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Extensions;
using Should;
using Xunit;

namespace Tests
{
    public class ExecutionProfile
    {
        [Fact]
        public async Task MinimalSchema()
        {
            var schema = GraphQL.Conventions.Schema.New()
                .WithQuery<Data.Query>();
            var result = await schema.Execute("{ __schema { queryType { name } } }");
            var expectedResult = "{\"data\":{\"__schema\":{\"queryType\":{\"name\":\"Query\"}}}}";
            result.ShouldEqual(expectedResult);

            var count = 1000;
            var delay = 1;
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < count; i++)
            {
                result = await schema.Execute("{ version }");
                result.ShouldContain("{\"data\":");
                Thread.Sleep(delay);
            }
            sw.Stop();
            var avgTime = (1.0 * sw.ElapsedMilliseconds / count) - delay;
            avgTime.ShouldBeLessThan(10);
        }

        class Data
        {
            #pragma warning disable 0649

            public class Query
            {
                public string Version = "1.0";
            }

            #pragma warning restore 0649
        }
    }
}
