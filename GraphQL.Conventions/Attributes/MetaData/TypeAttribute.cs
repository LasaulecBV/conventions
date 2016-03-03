using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    class TypeAttribute : MetaDataAttributeBase
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy;

        private readonly Type _overriddenType;
        private readonly bool _isInputType;

        public const int DefaultOrder = CoreAttribute.OverrideOrder;
        public const int InterjectionOrder = DefaultOrder + 1;
        public const int OverrideOrder = InterjectionOrder + 1;

        public TypeAttribute(Type type = null)
        {
            Order = OverrideOrder;
            _overriddenType = type ?? typeof(void);
        }

        internal TypeAttribute(bool isInputType)
        {
            Order = DefaultOrder;
            _isInputType = isInputType;
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (_overriddenType != null && _overriddenType != typeof(void))
            {
                entity.TypeRepresentation = _overriddenType;
            }

            if (entity.OriginalType == null || _overriddenType != null)
            {
                ParameterInfo parameterInfo;
                FieldInfo fieldInfo;
                PropertyInfo propertyInfo;
                MethodInfo methodInfo;
                TypeInfo typeInfo;

                if ((parameterInfo = entity.AttributeProvider as ParameterInfo) != null)
                {
                    entity.OriginalType = parameterInfo.ParameterType;
                    entity.IsNullable = !entity.OriginalType.IsValueType;
                    entity.Kind = Kind.Argument;
                }
                else if ((fieldInfo = entity.AttributeProvider as FieldInfo) != null)
                {
                    if (fieldInfo.IsLiteral)
                    {
                        entity.OriginalType = fieldInfo.DeclaringType;
                        entity.IsNullable = !entity.OriginalType.IsValueType;
                        entity.Kind = Kind.EnumValue;
                        entity.DefaultValue = Enum.Parse(
                            entity.TypeRepresentation ?? typeof(Enum),
                            (entity.AttributeProvider as MemberInfo).Name);
                    }
                    else
                    {
                        entity.OriginalType = fieldInfo.FieldType;
                        entity.IsNullable = !entity.OriginalType.IsValueType;
                        entity.Kind = Kind.Field;
                    }
                }
                else if ((propertyInfo = entity.AttributeProvider as PropertyInfo) != null)
                {
                    entity.OriginalType = propertyInfo.PropertyType;
                    entity.IsNullable = !entity.OriginalType.IsValueType;
                    entity.Kind = Kind.Field;
                }
                else if ((methodInfo = entity.AttributeProvider as MethodInfo) != null)
                {
                    entity.OriginalType = methodInfo.ReturnType;
                    entity.IsNullable = !entity.OriginalType.IsValueType;
                    entity.Kind = Kind.Field;
                    entity.Arguments.Clear();
                    entity.Arguments.AddRange(GetArguments(entity, methodInfo));
                }
                else if ((typeInfo = entity.AttributeProvider as TypeInfo) != null)
                {
                    entity.OriginalType = typeInfo;
                    entity.IsNullable = !entity.OriginalType.IsValueType;

                    if (typeInfo.IsInterface)
                    {
                        entity.Kind = Kind.InterfaceType;
                        entity.Fields.Clear();
                        entity.Fields.AddRange(GetFields(entity, entity.TypeRepresentation));
                    }
                    else if (typeInfo.IsEnum)
                    {
                        entity.Kind = Kind.EnumType;
                        entity.Fields.Clear();
                        entity.Fields.AddRange(GetEnumMembers(entity, entity.TypeRepresentation));
                    }
                    else
                    {
                        if (_overriddenType == null)
                        {
                            entity.Kind = _isInputType ? Kind.InputType : Kind.OutputType;
                        }
                        entity.Fields.Clear();
                        entity.Fields.AddRange(GetFields(entity, entity.TypeRepresentation));
                    }
                }
                else
                {
                    throw new ArgumentException(
                        "Unable to derive type for provided object",
                        nameof(entity.AttributeProvider));
                }
            }
        }

        private static IEnumerable<Entity> GetFields(Entity originatingEntity, Type type)
        {
            var fields = type.GetFields(DefaultBindingFlags)
                .Where(fieldInfo => !fieldInfo.IsSpecialName)
                .Where(IsValidMember);

            var properties = type.GetProperties(DefaultBindingFlags)
                .Where(propertyInfo => !propertyInfo.IsSpecialName)
                .Where(IsValidMember);

            var fieldEntities = type
                .GetMethods(DefaultBindingFlags)
                .Where(methodInfo => !methodInfo.IsSpecialName &&
                                     IsValidMember(methodInfo))
                .Cast<MemberInfo>()
                .Union(fields)
                .Union(properties)
                .Where(memberInfo => !memberInfo.DeclaringType.IsValueType)
                .Select(field => originatingEntity.Construct(field))
                .Where(field => !field.IsIgnored)
                .OrderBy(field => field.Name);

            foreach (var field in fieldEntities)
            {
                field.DeclaringType = originatingEntity;
                yield return field;
            }
        }

        private static bool IsValidMember(MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType.Namespace != nameof(System) &&
                   !memberInfo.DeclaringType.Namespace.StartsWith($"{nameof(System)}.") &&
                   !memberInfo.DeclaringType.IsValueType;
        }

        private static IEnumerable<Entity> GetEnumMembers(Entity originatingEntity, Type type)
        {
            foreach (var name in type.GetEnumNames())
            {
                var entity = originatingEntity.Construct(type
                    .GetMember(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .FirstOrDefault());

                if (!entity.IsIgnored)
                {
                    entity.DeclaringType = originatingEntity;
                    yield return entity;
                }
            }
        }

        private static IEnumerable<Entity> GetArguments(Entity originatingEntity, MethodInfo methodInfo)
        {
            foreach (var argument in methodInfo?
                .GetParameters()
                .Select(field => originatingEntity.Construct(field, true))
                .Where(entity => !entity.IsIgnored) ?? new Entity[0])
            {
                argument.DeclaringType = originatingEntity;
                yield return argument;
            }
        }
    }
}
