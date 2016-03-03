using System;
using GraphQL.Conventions.Attributes.Data;

namespace GraphQL.Conventions.Attributes.Interfaces
{
    public abstract class ExecutionFilterAttributeBase : Attribute, IExecutionFilterAttribute
    {
        public int Order { get; set; }

        public virtual bool IsEnabled(ExecutionContext context)
        {
            return true;
        }

        public virtual void AfterExecution(ExecutionContext context)
        {
        }

        public virtual void BeforeExecution(ExecutionContext context)
        {
        }

        public virtual bool ShouldExecute(ExecutionContext context)
        {
            return true;
        }
    }
}
