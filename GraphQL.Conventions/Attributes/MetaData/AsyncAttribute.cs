using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    class AsyncAttribute : MetaDataAttributeBase
    {
        public AsyncAttribute()
        {
            Order = TypeAttribute.InterjectionOrder;
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (entity.OriginalType.IsGenericType &&
                entity.OriginalType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                entity.IsAsynchronous = true;
                entity.OriginalType = entity.OriginalType.GenericTypeArguments.First();
                entity.TypeRepresentation = entity.OriginalType;
            }
        }
    }
}
