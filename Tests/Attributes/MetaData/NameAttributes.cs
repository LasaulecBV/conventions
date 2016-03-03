using Xunit;

namespace Tests.Attributes.MetaData
{
    public class NameAttributes
    {
        [Fact]
        public void AsTypeName()
        {
            string.Empty.ShouldEqualTypeName(null);
            string.Empty.ShouldEqualTypeName(string.Empty);
            "Type".ShouldEqualTypeName("type");
            "SomeField".ShouldEqualTypeName("someField");
            "SomeField".ShouldEqualTypeName("someField");
            "YetAnotherField".ShouldEqualTypeName("yetAnotherField");
            "BLAH".ShouldEqualTypeName("BLAH");
        }

        [Fact]
        public void AsFieldName()
        {
            string.Empty.ShouldEqualFieldName(null);
            string.Empty.ShouldEqualFieldName(string.Empty);
            "field".ShouldEqualFieldName("Field");
            "someField".ShouldEqualFieldName("someField");
            "someField".ShouldEqualFieldName("SomeField");
            "yetAnotherField".ShouldEqualFieldName("YetAnotherField");
            "bLAH".ShouldEqualFieldName("BLAH");
        }

        [Fact]
        public void AsEnumValue()
        {
            string.Empty.ShouldEqualEnumValue(null);
            string.Empty.ShouldEqualEnumValue(string.Empty);
            "ENUM".ShouldEqualEnumValue("Enum");
            "SOME_ENUM".ShouldEqualEnumValue("SomeEnum");
            "YET_ANOTHER_ENUM_VALUE".ShouldEqualEnumValue("YetAnotherEnumValue");
            "BLAH".ShouldEqualEnumValue("BLAH");
            "SOME_FIELD".ShouldEqualEnumValue("SOME_FIELD");
        }
    }
}
