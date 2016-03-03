using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Profiling
{
    public interface IProfiler
    {
        void EnterResolver(ExecutionContext context);

        void ExitResolver(ExecutionContext context);
    }
}
