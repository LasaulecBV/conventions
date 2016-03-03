using System;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class DescriptionAttribute : MetaDataAttributeBase
    {
        public const int DefaultOrder = NameAttribute.OverrideOrder;
        public const int OverrideOrder = DefaultOrder + 1;

        private readonly string _description;

        public DescriptionAttribute(string description)
        {
            Order = DefaultOrder;
            _description = description;
        }

        internal DescriptionAttribute()
            : this(null)
        {
        }

        public override void DeriveMetaData(Entity entity)
        {
            if (string.IsNullOrWhiteSpace(_description) && entity.Kind == Kind.Argument)
            {
                switch (entity.Name)
                {
                    case "first":
                        entity.Description =
                            "Specifies the number of edges to return starting from `after`, " +
                            "or the first entry if `after` is not specified.";
                        break;
                    case "last":
                        entity.Description =
                            "Specifies the number of edges to return counting reversely from " +
                            "`before`, or the last entry if `before` is not specified.";
                        break;
                    case "after":
                        entity.Description =
                            "Only look at connected edges with cursors greater than the value " +
                            "of `after`.";
                        break;
                    case "before":
                        entity.Description =
                            "Only look at connected edges with cursors smaller than the value " +
                            "of `before`.";
                        break;
                }
            }
            else
            {
                entity.Description = _description;
            }
        }
    }
}
