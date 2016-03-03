using System;
using System.Collections.Generic;
using GraphQL.Conventions.Profiling;
using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.Data
{
    public class ExecutionContext
    {
        internal ExecutionContext(Entity entity, ResolveFieldContext fieldResolutionContext)
        {
            Entity = entity;
            FieldResolutionContext = fieldResolutionContext;
            Profilers = new List<IProfiler>();
        }

        public ResolveFieldContext FieldResolutionContext { get; private set; }

        public object RootContext { get; private set; }

        public IList<IProfiler> Profilers { get; private set; }

        public Entity Entity { get; private set; }

        public object Result { get; set; }

        public Exception Exception { get; internal set; }

        public bool DidExecute { get; internal set; }

        public bool DidSucceed => DidExecute && Exception == null;
    }
}
