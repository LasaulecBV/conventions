using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Types.Relay
{
    [Description("Information about pagination in a connection.")]
    public class PageInfo<T>
    {
        [Description("When paginating forwards, are there more items?")]
        public bool HasNextPage { get; set; }

        [Description("When paginating backwards, are there more items?")]
        public bool HasPreviousPage { get; set; }

        [Description("When paginating backwards, the cursor to continue.")]
        public Cursor<T> StartCursor { get; set; }

        [Description("When paginating forwards, the cursor to continue.")]
        public Cursor<T> EndCursor { get; set; }
    }
}
