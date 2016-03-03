using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.ExecutionFilter;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Attributes.Relay;

namespace GraphQL.Conventions.Attributes.Collectors
{
    class ExecutionFilterCollector
    {
        private readonly IList<IExecutionFilterAttribute> _defaultAttributes = new List<IExecutionFilterAttribute>();

        public void AddDefaultAttribute(IExecutionFilterAttribute attribute)
        {
            _defaultAttributes.Add(attribute);
        }

        public IEnumerable<IExecutionFilterAttribute> CollectAttributes(ICustomAttributeProvider obj)
        {
            return CollectAttributesImplementation(obj).OrderBy(attribute => attribute.Order).ToList();
        }

        private IEnumerable<IExecutionFilterAttribute> CollectAttributesImplementation(ICustomAttributeProvider obj)
        {
            if (ProfilableAttribute.IsMethod(obj))
            {
                yield return new ProfilableAttribute();
            }

            if (RelayMutationAttribute.IsPotentialMutation(obj))
            {
                yield return new RelayMutationAttribute();
            }

            foreach (var attribute in _defaultAttributes)
            {
                yield return attribute;
            }

            foreach (var attribute in AttributeInspector.GetAttributes<IExecutionFilterAttribute>(obj))
            {
                yield return attribute;
            }
        }
    }
}
