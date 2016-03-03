using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.Data
{
    public sealed class ResolutionContext<T>
    {
        public ResolveFieldContext FieldResolutionContext { get; private set; }

        public T Data => FieldResolutionContext.RootValue is T
            ? (T)FieldResolutionContext.RootValue
            : default(T);
    }
}
