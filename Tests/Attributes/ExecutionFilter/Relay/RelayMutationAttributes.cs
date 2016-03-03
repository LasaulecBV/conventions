using Should;
using Xunit;

namespace Tests.Attributes.ExecutionFilter.Relay
{
    public class RelayMutationAttributes
    {
        [Fact]
        public void FromClientMutationIdArgument()
        {
            false.ShouldBeTrue();
        }

        [Fact]
        public void FromInputArgument()
        {
            false.ShouldBeTrue();
        }
    }
}
