using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Relay;
using GraphQL.Types;
using Should;
using Xunit;

namespace Tests.Types.Relay
{
    public class Cursors
    {
        [Fact]
        public void IsValueType()
        {
            var entity = Entity.New(typeof(Cursor<Foo>));

            entity.Name.ShouldEqual("FooCursor");
            entity.Description.ShouldEqual("Cursor used in pagination.");
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(string));
            entity.OriginalType.ShouldEqual(typeof(Cursor<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeFalse();

            entity.Fields.Count.ShouldEqual(0);

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        [Fact]
        public void InstantiatesFromString()
        {
            var entity = Entity.New(typeof(Cursor<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<StringGraphType>>();

            var input = "Rm9vMTIzNDU=";
            var output = (Cursor<Foo>)entity.WrappedType.Wrap(input);
            output.Index.ShouldEqual(12345);
        }

        [Fact]
        public void ReturnsString()
        {
            var entity = Entity.New(typeof(Cursor<Foo>));
            entity.GraphType.ShouldBeType<NonNullGraphType<StringGraphType>>();

            var input = new Cursor<Foo>(12345);
            var output = entity.WrappedType.Unwrap(input);
            output.ShouldEqual("Rm9vMTIzNDU=");
        }

        [Fact]
        public void InstantiatesConsistently()
        {
            for (var i = 0; i <= 100; i++)
            {
                var cursor1 = new Cursor<Foo>(i);
                var cursor2 = new Cursor<Foo>(i);
                var cursor3 = new Cursor<Foo>(i + 1);

                cursor1.Value.ShouldEqual(cursor2.Value);
                cursor1.Index.ShouldEqual(cursor2.Index);

                cursor1.Value.ShouldNotEqual(cursor3.Value);
                cursor1.Index.ShouldEqual(cursor3.Index - 1);

                cursor1.ShouldBeLessThan(cursor3);
                cursor1.ShouldBeLessThanOrEqualTo(cursor3);
            }
        }

        [Fact]
        public void DiscriminatesBetweenTypes()
        {
            for (var i = 0; i <= 10; i++)
            {
                var cursor1 = new Cursor<Foo>(1);
                var cursor2 = new Cursor<Bar>(1);
                cursor1.Value.ShouldNotEqual(cursor2.Value);
                cursor1.Index.ShouldEqual(cursor2.Index);
            }
        }

        [Fact]
        public void IsComparable()
        {
            var cursor1 = new Cursor<Foo>(1234);
            var cursor2 = new Cursor<Foo>(1235);
            var cursor3 = new Cursor<Foo>(4321);

            cursor1.ShouldBeLessThanOrEqualTo(cursor1);
            cursor1.ShouldBeLessThanOrEqualTo(cursor2);
            cursor1.ShouldBeLessThan(cursor2);
            cursor1.ShouldBeLessThan(cursor3);

            cursor2.ShouldBeGreaterThanOrEqualTo(cursor1);
            cursor2.ShouldBeGreaterThan(cursor1);
            cursor2.ShouldBeLessThanOrEqualTo(cursor2);
            cursor2.ShouldBeLessThanOrEqualTo(cursor3);
            cursor2.ShouldBeLessThan(cursor3);

            cursor3.ShouldBeGreaterThanOrEqualTo(cursor1);
            cursor3.ShouldBeGreaterThan(cursor1);
            cursor3.ShouldBeGreaterThanOrEqualTo(cursor2);
            cursor3.ShouldBeGreaterThan(cursor2);
            cursor3.ShouldBeGreaterThanOrEqualTo(cursor3);
        }

        [Fact]
        public void NegativeIndicesDefaultToZero()
        {
            var negativeCursor = new Cursor<Foo>(-1);
            var zeroCursor = new Cursor<Foo>(0);
            negativeCursor.ShouldEqual(zeroCursor);
        }

        private class Foo { }

        private class Bar { }
    }
}
