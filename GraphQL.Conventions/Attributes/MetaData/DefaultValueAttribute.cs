using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class DefaultValueAttribute : MetaDataAttributeBase
    {
        private readonly object _defaultValue;
        private readonly bool _setDefaultValue;

        public const int DefaultOrder = CoreAttribute.OverrideOrder;
        public const int OverrideOrder = DefaultOrder + 1;

        public DefaultValueAttribute(object defaultValue)
        {
            Order = OverrideOrder;
            _defaultValue = defaultValue;
            _setDefaultValue = true;
        }

        public DefaultValueAttribute()
        {
            Order = DefaultOrder;
            _setDefaultValue = false;
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (_setDefaultValue)
            {
                entity.DefaultValue = _defaultValue;
            }
            else
            {
                var paramInfo = entity.AttributeProvider as ParameterInfo;
                if (paramInfo != null && paramInfo.HasDefaultValue)
                {
                    entity.DefaultValue = paramInfo.DefaultValue;
                }
            }
        }
    }
}
