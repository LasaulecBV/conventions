using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.Collectors
{
    static class AttributeInspector
    {
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider obj)
            where TAttribute : IAttribute
        {
            var memberInfo = obj as MemberInfo;
            if (memberInfo != null)
            {
                var inheritedAttributes = Attribute.GetCustomAttributes(memberInfo, true).OfType<TAttribute>();
                if (inheritedAttributes != null)
                {
                    foreach (var attribute in inheritedAttributes)
                    {
                        yield return attribute;
                    }
                }
            }
            else
            {
                var attributes = obj?.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        yield return attribute;
                    }
                }
            }
        }
    }
}
