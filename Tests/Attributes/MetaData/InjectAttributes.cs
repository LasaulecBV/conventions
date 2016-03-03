using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class InjectAttributes
    {
        #pragma warning disable 0649

        [Fact]
        public void Plain()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.PlainParameter));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.Kind.ShouldEqual(Kind.Argument);
        }

        [Fact]
        public void Injected()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.InjectedParameter));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.Kind.ShouldEqual(Kind.Injection);
            entity.IsIgnored.ShouldBeTrue();
        }

        [Fact]
        public void AutomaticallyInjected()
        {
            var methodInfo = typeof(Foo).GetMethod(nameof(Foo.InjectedParameter));
            var parameterInfo = methodInfo.GetParameters().First();
            var entity = Entity.New(parameterInfo);
            entity.Kind.ShouldEqual(Kind.Injection);
            entity.IsIgnored.ShouldBeTrue();
        }

        class Foo
        {
            public void PlainParameter(
                string parameter)
            { }

            public void InjectedParameter(
                [Inject] string parameter)
            { }

            public void AutomaticallyInjectedParameter(
                ResolutionContext<object> parameter)
            { }
        }

        #pragma warning restore 0649
    }
}
