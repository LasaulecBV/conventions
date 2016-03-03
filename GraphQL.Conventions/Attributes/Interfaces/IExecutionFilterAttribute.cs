using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Attributes.Interfaces
{
    public interface IExecutionFilterAttribute : IAttribute
    {
        int Order { get; }

        bool IsEnabled(ExecutionContext context);

        void AfterExecution(ExecutionContext context);

        void BeforeExecution(ExecutionContext context);

        bool ShouldExecute(ExecutionContext context);
    }
}
