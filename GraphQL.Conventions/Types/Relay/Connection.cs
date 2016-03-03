using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Types.Relay
{
    [Description("Connection to related objects with relevant pagination information.")]
    public class Connection<T>
    {
        private IEnumerable<Tuple<long, T>> _collection;
        private PageInfo<T> _pageInfo;
        private List<Edge<T>> _edges;
        private int? _first;
        private int? _last;
        private Cursor<T>? _after;
        private Cursor<T>? _before;
        private long _minIndex;
        private long _maxIndex;
        private bool _hasBeenPaginated;
        private int? _totalCount;

        [Description(
            "A count of the total number of objects in this connection, ignoring pagination. This allows a " +
            "client to fetch the first five objects by passing \"5\" as the argument to first, then fetch " +
            "the total count so it could display \"5 of 83\", for example. In cases where we employ infinite " +
            "scrolling or don't have an exact count of entries, this field will return `null`.")]
        public int? TotalCount => _totalCount ?? _collection?.Count();

        [Description("Information to aid in pagination.")]
        public NonNull<PageInfo<T>> PageInfo
        {
            get
            {
                if (_pageInfo == null)
                {
                    Paginate(_first, _last, _after, _before);
                }
                return _pageInfo;
            }
        }

        [Description("Information to aid in pagination.")]
        public List<Edge<T>> Edges
        {
            get
            {
                if (_edges != null)
                {
                    return _edges;
                }

                _minIndex = 0;
                _maxIndex = 0;

                if (_collection == null)
                {
                    _edges = new List<Edge<T>>();
                    return _edges;
                }

                var edges = _collection
                    .Select(t => new Edge<T>
                    {
                        Cursor = new Cursor<T>(t.Item1),
                        Node = t.Item2,
                    });

                if (_after.HasValue)
                {
                    var lastEntryBefore = edges
                        .TakeWhile(edge => edge.Cursor <= _after.Value)
                        .LastOrDefault();

                    if (lastEntryBefore != null)
                    {
                        _minIndex = lastEntryBefore.Cursor;
                    }

                    edges = edges.SkipWhile(edge => edge.Cursor <= _after.Value);
                }

                if (_before.HasValue)
                {
                    var beforeEdges = edges
                        .TakeWhile(edge => edge.Cursor <= _before.Value)
                        .ToList();

                    _maxIndex = beforeEdges.LastOrDefault()?.Cursor ?? 0;

                    edges = beforeEdges
                        .TakeWhile(edge => edge.Cursor < _before.Value);
                }

                if (_first.HasValue)
                {
                    var edgesList = edges.Take(_first.Value + 1).ToList();
                    _maxIndex = edgesList.LastOrDefault()?.Cursor ?? 0;
                    edgesList = edgesList.Take(_first.Value).ToList();
                    edges = edgesList;
                }

                if (_last.HasValue)
                {
                    var edgesList = edges.ToList();
                    var count = edgesList.Count;
                    if (_last.Value < count)
                    {
                        _minIndex = edgesList
                            .Skip(Math.Max(0, count - _last.Value - 2))
                            .FirstOrDefault()?
                            .Cursor ?? 0;
                        edges = edgesList.Skip(count - _last.Value);
                    }
                    else
                    {
                        edges = edgesList;
                        _minIndex = edgesList.FirstOrDefault()?.Cursor ?? 0;
                    }
                }

                _edges = edges.ToList();

                if (_minIndex == 0)
                {
                    _minIndex = _collection.FirstOrDefault()?.Item1 ?? 0;
                }

                if (_maxIndex == 0)
                {
                    _maxIndex = _collection.Any() ? _collection.Max(n => n.Item1) : 0;
                }

                return _edges;
            }
        }

        [Description(
            "A list of all of the objects returned in the connection. This is a convenience field provided " +
            "for quickly exploring the API; rather than querying for `{ edges { node } }` when no edge data " +
            "is needed, this field can be used instead. Note that when clients like Relay need to fetch the " +
            "`cursor` field on the edge to enable efficient pagination, this shortcut cannot be used, and " +
            "the full `{ edges { node } }` version should be used instead.")]
        public List<T> Items => Edges?.Select(edge => edge != null ? edge.Node : default(T)).ToList();

        public static implicit operator Connection<T>(List<T> collection)
        {
            return new Connection<T>().FromCollection(collection);
        }

        [Ignore]
        public Connection<T> FromCollection(IEnumerable<Tuple<long, T>> collection)
        {
            _collection = collection;
            return this;
        }

        [Ignore]
        public Connection<T> FromCollection(IEnumerable<Tuple<DateTime, T>> collection)
        {
            FromCollection(collection.Select(item => Tuple.Create(-(new DateTimeOffset(item.Item1).ToUnixTimeMilliseconds()), item.Item2)));
            return this;
        }

        [Ignore]
        public Connection<T> FromCollection(IEnumerable<T> collection)
        {
            return FromCollection(collection.Select((item, index) => Tuple.Create((long)(index + 1), item)));
        }

        [Ignore]
        public Connection<T> FromEdges(
            IEnumerable<Edge<T>> edges, bool hasNextPage, bool hasPreviousPage, int? totalCount = null,
            Cursor<T> defaultCursorIfEmpty = default(Cursor<T>))
        {
            _edges = edges.ToList();
            _totalCount = totalCount;
            _hasBeenPaginated = true;
            _pageInfo = new PageInfo<T>
            {
                StartCursor = _edges.Any() ? _edges.Min(edge => edge.Cursor) : defaultCursorIfEmpty,
                EndCursor = _edges.Any() ? _edges.Max(edge => edge.Cursor) : defaultCursorIfEmpty,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
            };
            return this;
        }

        [Ignore]
        public Connection<T> Paginate(
            int? first = null, int? last = null, Cursor<T>? after = null, Cursor<T>? before = null)
        {
            if (_hasBeenPaginated)
            {
                throw new Exception("Cannot paginate connection twice.");
            }

            _first = first;
            _last = last;
            _after = after;
            _before = before;
            ValidateParameters();

            _edges = null;
            var edges = Edges;
            Cursor<T> minCursor = _minIndex;
            Cursor<T> maxCursor = _maxIndex;

            if (edges.Any())
            {
                var minPageCursor = edges.Min(edge => edge.Cursor);
                var maxPageCursor = edges.Max(edge => edge.Cursor);

                _pageInfo = new PageInfo<T>
                {
                    StartCursor = minPageCursor,
                    EndCursor = maxPageCursor,
                    HasNextPage = maxPageCursor < maxCursor,
                    HasPreviousPage = minPageCursor > minCursor,
                };
            }
            else
            {
                _pageInfo = new PageInfo<T>
                {
                    StartCursor = Cursor<T>.FromFirstNonNullString(after, before, minCursor),
                    EndCursor = Cursor<T>.FromFirstNonNullString(before, after, maxCursor),
                };

                long startValue = PageInfo.Value.StartCursor;
                long endValue = PageInfo.Value.EndCursor;

                if (before.HasValue)
                {
                    Cursor<T> cursorValue = Math.Min(startValue - 1, endValue - 1);
                    PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                }
                else if (after.HasValue)
                {
                    Cursor<T> cursorValue = Math.Max(startValue + 1, endValue + 1);
                    PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                }
                else if (first.HasValue)
                {
                    Cursor<T> cursorValue = Math.Min(startValue - 1, endValue - 1);
                    PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                }
                else if (last.HasValue)
                {
                    Cursor<T> cursorValue = Math.Max(startValue + 1, endValue + 1);
                    PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                }

                PageInfo.Value.HasNextPage =
                    (_first.GetValueOrDefault(-1) == 0)
                    ? PageInfo.Value.EndCursor <= maxCursor
                    : PageInfo.Value.EndCursor < maxCursor;

                PageInfo.Value.HasPreviousPage = PageInfo.Value.StartCursor > minCursor;
            }

            _hasBeenPaginated = true;
            return this;
        }

        [Ignore]
        public Connection<TNew> Transform<TNew>(
            Func<IEnumerable<T>, IDictionary<T, TNew>> transformerDelegate)
        {
            var connection = new Connection<TNew>
            {
                _collection = _collection.Select(t => Tuple.Create(t.Item1, default(TNew))),
                _edges = new List<Edge<TNew>>(),
                _pageInfo = new PageInfo<TNew>
                {
                    StartCursor = PageInfo.Value.StartCursor.Index,
                    EndCursor = PageInfo.Value.EndCursor.Index,
                    HasNextPage = PageInfo.Value.HasNextPage,
                    HasPreviousPage = PageInfo.Value.HasPreviousPage,
                },
                _hasBeenPaginated = true,
            };

            var transformedItems = transformerDelegate(Items);
            foreach (var edge in Edges)
            {
                TNew transformedItem;
                transformedItems.TryGetValue(edge.Node, out transformedItem);

                connection._edges.Add(new Edge<TNew>
                {
                    Cursor = new Cursor<TNew>(edge.Cursor.Index),
                    Node = transformedItem,
                });
            }

            return connection;
        }

        private void ValidateParameters()
        {
            if (_first.HasValue && _last.HasValue)
            {
                throw new ArgumentException("Cannot use `first` in conjunction with `last`.");
            }

            if (_after.HasValue && _before.HasValue)
            {
                throw new ArgumentException("Cannot use `after` in conjunction with `before`.");
            }

            if (_first.HasValue && _before.HasValue)
            {
                throw new ArgumentException("Cannot use `first` in conjunction with `before`.");
            }

            if (_last.HasValue && _after.HasValue)
            {
                throw new ArgumentException("Cannot use `last` in conjunction with `after`.");
            }
        }
    }
}
