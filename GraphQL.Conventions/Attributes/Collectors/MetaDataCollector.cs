using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.ExecutionFilter;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Attributes.Collectors
{
    class MetaDataCollector
    {
        private readonly List<IMetaDataAttribute> _defaultAttributes = new List<IMetaDataAttribute>();

        public MetaDataCollector()
        {
            AddDefaultAttribute(new AsyncAttribute());
            AddDefaultAttribute(new InterfaceAttribute());
            AddDefaultAttribute(new NameAttribute());
            AddDefaultAttribute(new DescriptionAttribute());
            AddDefaultAttribute(new DefaultValueAttribute());
            AddDefaultAttribute(new ProfilableAttribute());
            AddDefaultAttribute(new InjectAttribute(false));
            AddDefaultAttribute(new ConverterAttribute());
            AddDefaultAttribute(new ListAttribute());
            AddDefaultAttribute(new NonNullableAttribute(true));
            AddDefaultAttribute(new NullableAttribute());
        }

        public void AddDefaultAttribute(IMetaDataAttribute attribute)
        {
            _defaultAttributes.Add(attribute);
            _defaultAttributes.AddRange(attribute.AssociatedAttributes);
        }

        public List<IMetaDataAttribute> CollectAttributes(ICustomAttributeProvider obj, bool isInputType)
        {
            return CollectAttributesImplementation(obj, isInputType).OrderBy(attribute => attribute.Order).ToList();
        }

        private IEnumerable<IMetaDataAttribute> CollectAttributesImplementation(ICustomAttributeProvider obj, bool isInputType)
        {
            yield return new CoreAttribute(obj);
            yield return new TypeAttribute(isInputType);

            foreach (var attribute in _defaultAttributes)
            {
                yield return attribute;
            }

            foreach (var attribute in AttributeInspector.GetAttributes<IMetaDataAttribute>(obj))
            {
                yield return attribute;
                foreach (var associatedAttribute in attribute.AssociatedAttributes)
                {
                    yield return associatedAttribute;
                }
            }
        }
    }
}
