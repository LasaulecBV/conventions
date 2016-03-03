using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Types.Relay;
using Should;
using Xunit;

namespace Tests.Types.Relay
{
    public class Connections
    {
        [Fact]
        public void IsOutputObject()
        {
            var entity = Entity.New(typeof(Connection<Foo>));

            entity.Name.ShouldEqual("FooConnection");
            entity.Description.ShouldEqual("Connection to related objects with relevant pagination information.");
            entity.Kind.ShouldEqual(Kind.OutputType);
            entity.TypeRepresentation.ShouldEqual(typeof(Connection<Foo>));
            entity.IsDeprecated.ShouldBeFalse();
            entity.IsIgnored.ShouldBeFalse();
            entity.IsOutput.ShouldBeTrue();
            entity.IsInput.ShouldBeFalse();
            entity.IsNullable.ShouldBeTrue();

            entity.Fields.Count.ShouldEqual(4);
            entity.ShouldHaveField("totalCount");
            entity.ShouldHaveField("pageInfo");
            entity.ShouldHaveField("edges");
            entity.ShouldHaveField("items");

            var pageInfo = entity.Fields.First(field => field.Name == "pageInfo").WrappedType.Entity;
            pageInfo.Fields.Count.ShouldEqual(4);
            pageInfo.ShouldHaveField("hasNextPage");
            pageInfo.ShouldHaveField("hasPreviousPage");
            pageInfo.ShouldHaveField("startCursor");
            pageInfo.ShouldHaveField("endCursor");

            entity.Arguments.ShouldBeEmpty();
            entity.Interfaces.ShouldBeEmpty();
            entity.UnionTypes.ShouldBeEmpty();
            entity.ExecutionFilters.ShouldBeEmpty();
        }

        [Fact]
        public void WithoutPagination()
        {
            var connection = GenerateConnection(5);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count.ShouldEqual(5);
            connection.Items.Count.ShouldEqual(5);

            for (var i = 1; i <= 5; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor(i));
                connection.Edges[i - 1].Cursor.Value.ShouldNotEqual(Cursor<int>(i).Value);
                connection.Edges[i - 1].Node.Value.ShouldEqual(i);
                connection.Items[i - 1].Value.ShouldEqual(i);
                connection.Edges[i - 1].Cursor.Index.ShouldEqual(i);
            }
        }

        [Fact]
        public void First3()
        {
            var connection = GenerateConnection(5, first: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor(i));
                connection.Edges[i - 1].Node.Value.ShouldEqual(i);
                connection.Items[i - 1].Value.ShouldEqual(i);
            }
        }

        [Fact]
        public void Last3()
        {
            var connection = GenerateConnection(5, last: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor(i + 2));
                connection.Edges[i - 1].Node.Value.ShouldEqual(i + 2);
                connection.Items[i - 1].Value.ShouldEqual(i + 2);
            }
        }

        [Fact]
        public void First3After1()
        {
            var connection = GenerateConnection(5, after: Cursor(1), first: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(4));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor(i + 1));
                connection.Edges[i - 1].Node.Value.ShouldEqual(i + 1);
                connection.Items[i - 1].Value.ShouldEqual(i + 1);
            }
        }

        [Fact]
        public void Last3Before5()
        {
            var connection = GenerateConnection(5, before: Cursor(5), last: 3);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(4));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor(i + 1));
                connection.Edges[i - 1].Node.Value.ShouldEqual(i + 1);
                connection.Items[i - 1].Value.ShouldEqual(i + 1);
            }
        }

        [Fact]
        public void AfterBounds()
        {
            var connection = GenerateConnection(5, after: Cursor(5), first: 2);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(6));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(6));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }


        [Fact]
        public void BeforeBounds()
        {
            var connection = GenerateConnection(5, before: Cursor(1), last: 2);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void First0()
        {
            var connection = GenerateConnection(5, first: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void First0After2()
        {
            var connection = GenerateConnection(5, after: Cursor(2), first: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void First1After4()
        {
            var connection = GenerateConnection(5, after: Cursor(4), first: 1);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(5));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(5));
            connection.Edges.Count.ShouldEqual(1);
            connection.Items.Count.ShouldEqual(1);
            connection.Edges[0].Cursor.ShouldEqual(Cursor(5));
            connection.Edges[0].Node.Value.ShouldEqual(5);
            connection.Items[0].Value.ShouldEqual(5);
        }

        [Fact]
        public void First1After2()
        {
            var connection = GenerateConnection(5, after: Cursor(2), first: 1);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(3));
            connection.Edges.Count.ShouldEqual(1);
            connection.Items.Count.ShouldEqual(1);
            connection.Edges[0].Cursor.ShouldEqual(Cursor(3));
            connection.Edges[0].Node.Value.ShouldEqual(3);
            connection.Items[0].Value.ShouldEqual(3);
        }

        [Fact]
        public void AfterLast()
        {
            var connection = GenerateConnection(5, after: Cursor(5));

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(6));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(6));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void Last0()
        {
            var connection = GenerateConnection(5, last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(6));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(6));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void Last0Before2()
        {
            var connection = GenerateConnection(5, before: Cursor(2), last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(1));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void Last0Before3()
        {
            var connection = GenerateConnection(5, before: Cursor(3), last: 0);

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(2));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void BeforeFirst()
        {
            var connection = GenerateConnection(5, before: Cursor(1));

            connection.TotalCount.ShouldEqual(5);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor(0));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void CombineBeforeAndAfter()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, before: Cursor(1), after: Cursor(1));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `after` in conjunction with `before`.");
        }

        [Fact]
        public void CombineFirstAndLast()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, first: 1, last: 1);
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `first` in conjunction with `last`.");
        }

        [Fact]
        public void CombineFirstAndBefore()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, first: 1, before: Cursor(3));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `first` in conjunction with `before`.");
        }

        [Fact]
        public void CombineLastAndAfter()
        {
            ArgumentException argEx = null;
            try
            {
                GenerateConnection(5, last: 1, after: Cursor(1));
            }
            catch (ArgumentException ex)
            {
                argEx = ex;
            }
            argEx.ShouldNotBeNull();
            argEx?.Message.ShouldEqual("Cannot use `last` in conjunction with `after`.");
        }

        [Fact]
        public void TransformFirst3After1()
        {
            var connection = GenerateConnection(6, first: 3, after: Cursor(1));
            var transformedConnection = connection.Transform(items =>
                items.ToDictionary(
                    foo => foo,
                    foo => new Bar { Value = foo.Value.ToString("D4") }));

            transformedConnection.TotalCount.ShouldEqual(6);
            transformedConnection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            transformedConnection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            transformedConnection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<Bar>(2));
            transformedConnection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<Bar>(4));
            transformedConnection.Edges.Count.ShouldEqual(3);
            transformedConnection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                transformedConnection.Edges[i - 1].Cursor.ShouldEqual(Cursor<Bar>(i + 1));
                transformedConnection.Edges[i - 1].Node.Value.ShouldEqual($"{i + 1:D4}");
                transformedConnection.Items[i - 1].Value.ShouldEqual($"{i + 1:D4}");
            }
        }

        [Fact]
        public void First3OfInfiniteCollection()
        {
            var connection = GenerateInfiniteConnection(5, 6, first: 3);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(3));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor<int>(i));
                connection.Edges[i - 1].Node.ShouldEqual(i);
            }
        }

        [Fact]
        public void First3After2OfInfiniteCollection()
        {
            var connection = GenerateInfiniteConnection(5, 6, first: 3, after: Cursor<int>(2));

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(5));
            connection.Edges.Count.ShouldEqual(3);
            connection.Items.Count.ShouldEqual(3);

            for (var i = 1; i <= 3; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor<int>(i + 2));
                connection.Edges[i - 1].Node.ShouldEqual(i + 2);
            }
        }

        [Fact]
        public void Last3OfInfiniteCollectionThrowsException()
        {
            try
            {
                GenerateInfiniteConnection(5, 6, last: 3);
                true.ShouldBeFalse("Getting the last entries of an infinite collection should fail.");
            }
            catch (Exception ex)
            {
                ex.Message.ShouldEqual("Enumerated too many entries");
            }
        }

        [Fact]
        public void TotalCountOfInfiniteCollectionThrowsException()
        {
            try
            {
                var connection = GenerateInfiniteConnection(5, 6, first: 3);
                connection.TotalCount.ShouldBeNull();
                true.ShouldBeFalse("Getting the total count of an infinite collection should fail.");
            }
            catch (Exception ex)
            {
                ex.Message.ShouldEqual("Enumerated too many entries");
            }
        }

        [Fact]
        public void FromEdges()
        {
            var edges = new List<Edge<int>>
            {
                new Edge<int> { Cursor = Cursor<int>(1), Node = 1 },
                new Edge<int> { Cursor = Cursor<int>(2), Node = 2 },
            };
            var connection = new Connection<int>()
                .FromEdges(edges, true, false);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(2));
            connection.Edges.Count.ShouldEqual(2);
            connection.Items.Count.ShouldEqual(2);
            connection.TotalCount.ShouldEqual(null);

            for (var i = 1; i <= 2; i++)
            {
                connection.Edges[i - 1].Cursor.ShouldEqual(Cursor<int>(i));
                connection.Edges[i - 1].Node.ShouldEqual(i);
            }
        }

        [Fact]
        public void FromEdgesWithCount()
        {
            var edges = new List<Edge<int>>
            {
                new Edge<int> { Cursor = Cursor<int>(2), Node = 2 },
                new Edge<int> { Cursor = Cursor<int>(3), Node = 3 },
                new Edge<int> { Cursor = Cursor<int>(4), Node = 4 },
                new Edge<int> { Cursor = Cursor<int>(5), Node = 5 },
            };
            var connection = new Connection<int>()
                .FromEdges(edges, true, true, 10);

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(2));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(5));
            connection.Edges.Count.ShouldEqual(4);
            connection.Items.Count.ShouldEqual(4);
            connection.TotalCount.ShouldEqual(10);
        }

        [Fact]
        public void FromEdgesEmpty()
        {
            var connection = new Connection<int>()
                .FromEdges(new List<Edge<int>>(), true, false, null, Cursor<int>(4));

            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(4));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(4));
            connection.Edges.Count.ShouldEqual(0);
            connection.Items.Count.ShouldEqual(0);
            connection.TotalCount.ShouldEqual(null);
        }

        [Fact]
        public void FromFilteredEnumerable()
        {
            var enumerable = Sequence(10)
                .Where(t => t.Item1 % 2 == 1);

            var collection = new Connection<int>()
                .FromCollection(enumerable);

            collection.TotalCount.ShouldEqual(5);

            collection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            collection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            collection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            collection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(9));

            collection.Edges[0].Cursor.ShouldEqual(Cursor<int>(1));
            collection.Edges[1].Cursor.ShouldEqual(Cursor<int>(3));
            collection.Edges[2].Cursor.ShouldEqual(Cursor<int>(5));
            collection.Edges[3].Cursor.ShouldEqual(Cursor<int>(7));
            collection.Edges[4].Cursor.ShouldEqual(Cursor<int>(9));

            collection.Edges[0].Node.ShouldEqual(1 * 2);
            collection.Edges[1].Node.ShouldEqual(3 * 2);
            collection.Edges[2].Node.ShouldEqual(5 * 2);
            collection.Edges[3].Node.ShouldEqual(7 * 2);
            collection.Edges[4].Node.ShouldEqual(9 * 2);
        }


        [Fact]
        public void FromFilteredEnumerableFirstN()
        {
            var enumerable = Sequence(10)
                .Where(t => t.Item1 % 2 == 1);

            var collection = new Connection<int>()
                .FromCollection(enumerable)
                .Paginate(10);

            collection.TotalCount.ShouldEqual(5);

            collection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            collection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            collection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<int>(1));
            collection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<int>(9));

            collection.Edges[0].Cursor.ShouldEqual(Cursor<int>(1));
            collection.Edges[1].Cursor.ShouldEqual(Cursor<int>(3));
            collection.Edges[2].Cursor.ShouldEqual(Cursor<int>(5));
            collection.Edges[3].Cursor.ShouldEqual(Cursor<int>(7));
            collection.Edges[4].Cursor.ShouldEqual(Cursor<int>(9));

            collection.Edges[0].Node.ShouldEqual(1 * 2);
            collection.Edges[1].Node.ShouldEqual(3 * 2);
            collection.Edges[2].Node.ShouldEqual(5 * 2);
            collection.Edges[3].Node.ShouldEqual(7 * 2);
            collection.Edges[4].Node.ShouldEqual(9 * 2);
        }

        [Fact]
        public void FromSingleTuple()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(1, "onlyEntry"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate();

            connection.TotalCount.ShouldEqual(1);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(1));
        }

        [Fact]
        public void FromMultipleTuples()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(1, "entry1"),
                new Tuple<long, string>(3, "entry2"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate();

            connection.TotalCount.ShouldEqual(2);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(1));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(3));
        }

        [Fact]
        public void FromSingleTupleWithOffset()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "onlyEntry"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate();

            connection.TotalCount.ShouldEqual(1);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(3));
        }

        [Fact]
        public void FromMultipleTuplesWithOffset()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "entry1"),
                new Tuple<long, string>(4, "entry2"),
                new Tuple<long, string>(7, "entry3"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate();

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(7));
        }

        [Fact]
        public void FromMultipleTuplesWithOffsetAndPaginationFirst2()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "entry1"),
                new Tuple<long, string>(5, "entry2"),
                new Tuple<long, string>(7, "entry3"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate(first: 2);

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(5));
            connection.Items.Count.ShouldEqual(2);
            connection.Edges[0].Cursor.ShouldEqual(Cursor<string>(3));
            connection.Edges[0].Node.ShouldEqual("entry1");
            connection.Edges[1].Cursor.ShouldEqual(Cursor<string>(5));
            connection.Edges[1].Node.ShouldEqual("entry2");
        }

        [Fact]
        public void FromMultipleTuplesWithOffsetAndPaginationFirst2After1()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "entry1"),
                new Tuple<long, string>(5, "entry2"),
                new Tuple<long, string>(7, "entry3"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate(first: 2, after: Cursor<string>(3));

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(5));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(7));
            connection.Items.Count.ShouldEqual(2);
            connection.Edges[0].Cursor.ShouldEqual(Cursor<string>(5));
            connection.Edges[0].Node.ShouldEqual("entry2");
            connection.Edges[1].Cursor.ShouldEqual(Cursor<string>(7));
            connection.Edges[1].Node.ShouldEqual("entry3");
        }

        [Fact]
        public void FromMultipleTuplesWithOffsetAndPaginationLast2()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "entry1"),
                new Tuple<long, string>(5, "entry2"),
                new Tuple<long, string>(7, "entry3"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate(last: 2);

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(true);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(5));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(7));
            connection.Items.Count.ShouldEqual(2);
            connection.Edges[0].Cursor.ShouldEqual(Cursor<string>(5));
            connection.Edges[0].Node.ShouldEqual("entry2");
            connection.Edges[1].Cursor.ShouldEqual(Cursor<string>(7));
            connection.Edges[1].Node.ShouldEqual("entry3");
        }

        [Fact]
        public void FromMultipleTuplesWithOffsetAndPaginationLast2Before1()
        {
            var items = new List<Tuple<long, string>>
            {
                new Tuple<long, string>(3, "entry1"),
                new Tuple<long, string>(5, "entry2"),
                new Tuple<long, string>(7, "entry3"),
            };
            var connection = new Connection<string>()
                .FromCollection(items)
                .Paginate(last: 2, before: Cursor<string>(7));

            connection.TotalCount.ShouldEqual(3);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(true);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(3));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(5));
            connection.Items.Count.ShouldEqual(2);
            connection.Edges[0].Cursor.ShouldEqual(Cursor<string>(3));
            connection.Edges[0].Node.ShouldEqual("entry1");
            connection.Edges[1].Cursor.ShouldEqual(Cursor<string>(5));
            connection.Edges[1].Node.ShouldEqual("entry2");
        }

        [Fact]
        public void FromEmptyCollection()
        {
            var connection = new Connection<string>()
                .FromCollection(new List<Tuple<long, string>>())
                .Paginate();

            connection.TotalCount.ShouldEqual(0);
            connection.PageInfo.Value.HasPreviousPage.ShouldEqual(false);
            connection.PageInfo.Value.HasNextPage.ShouldEqual(false);
            connection.PageInfo.Value.StartCursor.ShouldEqual(Cursor<string>(0));
            connection.PageInfo.Value.EndCursor.ShouldEqual(Cursor<string>(0));
            connection.Items.Count.ShouldEqual(0);
            connection.Edges.Count.ShouldEqual(0);
        }

        private IEnumerable<Tuple<long, int>> Sequence(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                yield return Tuple.Create((long)i, i * 2);
            }
        }

        private Cursor<Foo> Cursor(long index)
        {
            return Cursor<Foo>(index);
        }

        private Cursor<T> Cursor<T>(long index)
        {
            return new Cursor<T>(index);
        }

        private Connection<Foo> GenerateConnection(
            int count, int? first = null, int? last = null, Cursor<Foo>? after = null, Cursor<Foo>? before = null)
        {
            var items = new List<Foo>();
            for (var i = 1; i <= count; i++)
            {
                items.Add(new Foo { Value = i });
            }

            return new Connection<Foo>()
                .FromCollection(items)
                .Paginate(first, last, after, before);
        }

        private static IEnumerable<int> InfiniteCollection(int throwAboveThisNumber = -1)
        {
            for (var i = 1; ; i++)
            {
                if (throwAboveThisNumber >= 0 && i > throwAboveThisNumber)
                {
                    throw new Exception("Enumerated too many entries");
                }
                yield return i;
            }
        }

        private Connection<int> GenerateInfiniteConnection(
            int count, int throwExceptionAboveThisNumber,
            int? first = null, int? last = null, Cursor<int>? after = null, Cursor<int>? before = null)
        {
            return new Connection<int>()
                .FromCollection(InfiniteCollection(throwExceptionAboveThisNumber))
                .Paginate(first, last, after, before);
        }

        private class Foo
        {
            public int Value { get; set; }
        }

        private class Bar
        {
            public string Value { get; set; }
        }
    }
}
