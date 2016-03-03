using System;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.Handlers
{
    class ExecutionFilterHandler
    {
        public ExecutionContext Execute(Entity entity, ResolveFieldContext fieldResolutionContext, Func<ResolveFieldContext, object> executor)
        {
            var executionContext = new ExecutionContext(entity, fieldResolutionContext)
            {
                Result = null,
                Exception = null,
                DidExecute = false,
            };

            var filterAttributes = entity
                .ExecutionFilters
                .Where(attribute => attribute.IsEnabled(executionContext))
                .ToList();

            if (Enumerable.Any(filterAttributes, attribute => !attribute.ShouldExecute(executionContext)))
            {
                return executionContext;
            }

            ProtectedInvocation(executionContext, () =>
            {
                foreach (var attribute in filterAttributes)
                {
                    attribute.BeforeExecution(executionContext);
                }
            });

            ProtectedInvocation(executionContext, () =>
            {
                executionContext.Result = executor(executionContext.FieldResolutionContext);
                executionContext.DidExecute = true;
            });

            ProtectedInvocation(executionContext, () =>
            {
                foreach (var attribute in filterAttributes.Reverse<IExecutionFilterAttribute>())
                {
                    attribute.AfterExecution(executionContext);
                }
            });

            return executionContext;
        }

        private void ProtectedInvocation(ExecutionContext executionContext, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                executionContext.Exception = ex;
            }
        }
    }
}
