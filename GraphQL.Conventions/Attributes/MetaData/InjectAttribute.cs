using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class InjectAttribute : MetaDataAttributeBase
    {
        private readonly bool _isInjection;

        public InjectAttribute()
            : this(true)
        {
        }

        internal InjectAttribute(bool isInjection)
        {
            _isInjection = isInjection;
        }

        private static bool IsGenericType(Type type, Type genericTypeDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        public override void DeriveMetaData(Entity entity)
        {
            var isInjection = _isInjection;
            if (!isInjection && IsGenericType(entity.OriginalType, typeof(ResolutionContext<>)))
            {
                isInjection = true;
            }
            if (isInjection)
            {
                entity.Kind = Kind.Injection;
                entity.IsIgnored = true;
            }
        }
    }
}
