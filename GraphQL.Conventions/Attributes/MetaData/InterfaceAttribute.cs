using System;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class InterfaceAttribute : MetaDataAttributeBase
    {
        private readonly Type[] _interfaces = null;

        public InterfaceAttribute(params Type[] interfaces)
        {
            _interfaces = interfaces;
        }

        internal InterfaceAttribute()
        {
            Order = TypeAttribute.DefaultOrder;
        }

        public override void DeriveMetaData(Entity entity)
        {
            var interfaces = _interfaces;
            if (interfaces == null)
            {
                var typeInfo = entity.AttributeProvider as TypeInfo;
                interfaces = (typeInfo != null && !typeInfo.IsValueType)
                    ? typeInfo.GetInterfaces()
                    : new Type[0];
            }

            var interfaceEntities = interfaces
                .Where(IsValidInterface)
                .Select(type => entity.Construct(type))
                .Where(interfaceTypeEntity => !interfaceTypeEntity.IsIgnored)
                .ToList();

            entity.Interfaces.AddRange(interfaceEntities);

            foreach (var interfaceEntity in interfaceEntities)
            {
                if (!interfaceEntity.PossibleTypes.Contains(entity))
                {
                    interfaceEntity.PossibleTypes.Add(entity);
                }
            }
        }

        private static bool IsValidInterface(Type iface)
        {
            return !iface.Namespace.StartsWith($"{nameof(System)}.");
        }
    }
}
