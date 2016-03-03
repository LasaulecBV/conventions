using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Types;
using Should;

namespace Tests
{
    static class TestUtilities
    {
        public static void ShouldEqualTypeName(this string actual, string expected)
        {
            actual.ShouldEqual(NameAttribute.AsTypeName(expected));
        }

        public static void ShouldEqualFieldName(this string actual, string expected)
        {
            actual.ShouldEqual(NameAttribute.AsFieldName(expected));
        }

        public static void ShouldEqualEnumValue(this string actual, string expected)
        {
            actual.ShouldEqual(NameAttribute.AsEnumValue(expected));
        }

        public static void ShouldContain<T>(this List<Entity> entities)
        {
            entities.Any(entity => entity.TypeRepresentation == typeof(T)).ShouldBeTrue();
        }

        public static Entity ShouldHaveField(this Entity entity, string fieldName)
        {
            return entity.Fields.FirstOrDefault(field => field.Name == fieldName).ShouldNotBeNull();
        }

        public static FieldType ShouldHaveField(this GraphType graphType, string fieldName)
        {
            return graphType.Fields.FirstOrDefault(field => field.Name == fieldName).ShouldNotBeNull();
        }

        public static string Clean(this string str)
        {
            var lines = str
                .Split('\n')
                .Where(line => line.Contains('|'))
                .Select(line => line.Substring(line.IndexOf('|') + 2));
            return string.Join("\n", lines);
        }
    }
}
