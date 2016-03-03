using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class NameAttribute : MetaDataAttributeBase
    {
        public const int DefaultOrder = TypeAttribute.OverrideOrder + 1;
        public const int OverrideOrder = DefaultOrder + 1;

        private const string InputSuffix = "Input";

        private readonly string _name;

        public NameAttribute(string name)
        {
            _name = name;
        }

        internal NameAttribute()
            : this(null)
        {
            Order = DefaultOrder;
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (!string.IsNullOrWhiteSpace(_name))
            {
                entity.Name = _name;

                if (entity.AttributeProvider is TypeInfo && entity.IsInput && !entity.IsOutput)
                {
                    entity.Name += InputSuffix;
                }

                return;
            }

            var parameterInfo = entity.AttributeProvider as ParameterInfo;
            if (parameterInfo != null)
            {
                entity.Name = AsFieldName(parameterInfo.Name);
                return;
            }

            var memberInfo = entity.AttributeProvider as MemberInfo;
            var fieldInfo = entity.AttributeProvider as FieldInfo;
            if (fieldInfo != null)
            {
                entity.Name = fieldInfo.IsLiteral
                    ? AsEnumValue(memberInfo.Name)
                    : AsFieldName(memberInfo.Name);
                return;
            }

            if (memberInfo is PropertyInfo ||
                memberInfo is MethodInfo)
            {
                entity.Name = AsFieldName(memberInfo.Name);
                return;
            }

            var typeInfo = entity.AttributeProvider as TypeInfo;
            if (typeInfo != null)
            {
                if (typeInfo.IsGenericType)
                {
                    var types = typeInfo
                        .GenericTypeArguments
                        .Select(type => NormalizeTypeName(type.Name));
                    entity.Name = AsTypeName(string.Join(string.Empty, types) + NormalizeTypeName(typeInfo.Name));
                }
                else
                {
                    entity.Name = AsTypeName(typeInfo.Name);
                }

                if (entity.IsInput && !entity.IsOutput)
                {
                    entity.Name += InputSuffix;
                }

                return;
            }

            throw new ArgumentException("Unable to derive name for provided object", nameof(entity.AttributeProvider));
        }

        internal static string AsTypeName(string name)
        {
            name = NormalizeString(name);
            return name.Length > 0
                ? $"{char.ToUpperInvariant(name[0])}{name.Substring(1)}"
                : string.Empty;
        }

        private static string NormalizeTypeName(string name)
        {
            return name.Contains("`")
                ? name.Substring(0, name.IndexOf('`'))
                : name;
        }

        internal static string AsFieldName(string name)
        {
            name = NormalizeString(name);
            return name.Length > 0
                ? $"{char.ToLowerInvariant(name[0])}{name.Substring(1)}"
                : string.Empty;
        }

        internal static string AsParameterName(string name)
        {
            return AsFieldName(name);
        }

        internal static string AsEnumValue(string name)
        {
            return Regex
                .Replace(NormalizeString(name), @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4")
                .ToUpperInvariant();
        }
        
        private static string NormalizeString(string str)
        {
            str = str?.Trim();
            return string.IsNullOrWhiteSpace(str)
                ? string.Empty
                : str;
        }
    }
}
