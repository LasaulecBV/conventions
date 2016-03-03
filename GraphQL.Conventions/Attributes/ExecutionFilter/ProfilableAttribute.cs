using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Attributes.ExecutionFilter
{
    class ProfilableAttribute : ExecutionFilterAttributeBase, IMetaDataAttribute
    {
        internal static bool IsMethod(ICustomAttributeProvider obj)
        {
            return obj is MethodInfo;
        }

        private readonly bool _isEnabled;

        public ProfilableAttribute(bool isEnabled)
        {
            Order = CoreAttribute.OverrideOrder;
            _isEnabled = isEnabled;
        }

        internal ProfilableAttribute()
            : this(true)
        {
        }

        public List<IMetaDataAttribute> AssociatedAttributes { get; } = new List<IMetaDataAttribute>();

        public virtual bool ShouldBeApplied(Entity entity)
        {
            return true;
        }

        public override bool IsEnabled(ExecutionContext context)
        {
            return context.Entity.IsProfilable && context.Profilers.Any();
        }

        public override void AfterExecution(ExecutionContext context)
        {
            foreach (var profiler in context.Profilers.Reverse())
            {
                profiler.ExitResolver(context);
            }
        }

        public override void BeforeExecution(ExecutionContext context)
        {
            foreach (var profiler in context.Profilers)
            {
                profiler.EnterResolver(context);
            }
        }

        public void DeriveMetaData(Entity entity)
        {
            entity.IsProfilable = IsMethod(entity.AttributeProvider) && _isEnabled;
        }
    }
}
