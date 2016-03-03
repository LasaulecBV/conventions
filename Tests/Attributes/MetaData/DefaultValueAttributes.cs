using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class DefaultValueAttributes
    {
        #pragma warning disable 0649

        [Fact]
        public void StringParameterWithoutDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.StringParameterWithoutDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual(null);
        }

        [Fact]
        public void StringParameterWithDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.StringParameterWithDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual("something");
        }

        [Fact]
        public void StringParameterWithExplicitDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.StringParameterWithExplicitDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual("something");
        }

        [Fact]
        public void IntegerParameterWithoutDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.IntegerParameterWithoutDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual(null);
        }

        [Fact]
        public void IntegerParameterWithDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.IntegerParameterWithDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual(99);
        }

        [Fact]
        public void IntegerParameterWithExplicitDefault()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.IntegerParameterWithExplicitDefault));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.DefaultValue.ShouldEqual(99);
        }

        class Foo
        {
            public void StringParameterWithoutDefault(
                string parameter)
            { }

            public void StringParameterWithDefault(
                [DefaultValue("something")] string parameter)
            { }

            public void StringParameterWithExplicitDefault(
                string parameter = "something")
            { }

            public void IntegerParameterWithoutDefault(
                int parameter)
            { }

            public void IntegerParameterWithDefault(
                [DefaultValue(99)] int parameter)
            { }

            public void IntegerParameterWithExplicitDefault(
                int parameter = 99)
            { }
        }

        #pragma warning restore 0649
    }
}
