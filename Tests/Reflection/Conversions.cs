using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types;
using Should;
using Xunit;

namespace Tests.Reflection
{
    public class Conversions
    {
        [Fact]
        public void StringToNonNullableString()
        {
            var value = Conversion.ConvertImplicity("test", typeof(string), typeof(NonNull<string>));
            value.ShouldBeType<NonNull<string>>().Value.ShouldEqual("test");
        }

        [Fact]
        public void NonNullableStringToString()
        {
            var value = Conversion.ConvertImplicity(new NonNull<string>("test"), typeof(NonNull<string>), typeof(string));
            value.ShouldBeType<string>().ShouldEqual("test");
        }
    }
}
