using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Handlers;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Types;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    class CoreAttribute : MetaDataAttributeBase
    {
        public const int DefaultOrder = IgnoreAttribute.DefaultOrder + 1;
        public const int OverrideOrder = DefaultOrder + 1;

        private static readonly ExecutionFilterHandler _executionFilterHandler = new ExecutionFilterHandler();

        private readonly ICustomAttributeProvider _attributeProvider;

        public CoreAttribute(ICustomAttributeProvider attributeProvider)
        {
            Order = DefaultOrder;
            _attributeProvider = attributeProvider;
        }

        public override void DeriveMetaData(Entity entity)
        {
            entity.AttributeProvider = _attributeProvider;
            Func<ResolveFieldContext, object> resolver = null;

            if (entity.ExecutionFilters == null)
            {
                throw new ArgumentNullException(
                    nameof(entity.ExecutionFilters),
                    "Execution filters must be derived before constructing resolvers");
            }

            if (_attributeProvider is MethodInfo)
            {
                resolver = context =>
                {
                    var methodInfo = _attributeProvider as MethodInfo;
                    var obj = context.Source ?? entity.TypeConstructor.CreateInstance(methodInfo.DeclaringType);
                    object[] args = new object[0];
                    return methodInfo.Invoke(obj, args);
                };
            }
            else if (_attributeProvider is PropertyInfo)
            {
                resolver = context =>
                {
                    var propertyInfo = _attributeProvider as PropertyInfo;
                    var obj = context.Source ?? entity.TypeConstructor.CreateInstance(propertyInfo.DeclaringType);
                    return propertyInfo.GetValue(obj);
                };
            }
            else if (_attributeProvider is FieldInfo)
            {
                resolver = context =>
                {
                    var fieldInfo = _attributeProvider as FieldInfo;
                    var obj = context.Source ?? entity.TypeConstructor.CreateInstance(fieldInfo.DeclaringType);
                    return ((FieldInfo)_attributeProvider).GetValue(obj);
                };
            }

            if (resolver != null)
            {
                entity.Resolver = context =>
                {
                    var executionContext = _executionFilterHandler.Execute(entity, context, resolver);
                    if (executionContext.DidSucceed)
                    {
                        return executionContext.Result;
                    }
                    else
                    {
                        throw executionContext.Exception;
                    }
                };
            }
        }
    }
}
