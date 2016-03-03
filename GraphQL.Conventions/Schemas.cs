using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace GraphQL.Conventions
{
    public class Schemas
    {
        private readonly List<ISchema> _schemas = new List<ISchema>();
        private ObjectGraphType _queryObject;
        private ObjectGraphType _mutationObject;

        public static Schemas New(ISchema schema = null)
        {
            return new Schemas().With(schema);
        }

        public Schemas With(ISchema schema)
        {
            _schemas.Add(schema);
            return this;
        }

        public ISchema Combine()
        {
            foreach (var schema in _schemas.Where(schema => schema != null))
            {
                if (schema.Query != null)
                {
                    EnsureObjectExists(ref _queryObject, "Query", "Queries.");
                    foreach (var field in schema.Query.Fields)
                    {
                        AddFieldToType(_queryObject, field);
                    }
                }

                if (schema.Mutation != null)
                {
                    EnsureObjectExists(ref _mutationObject, "Mutation", "Mutations.");
                    foreach (var field in schema.Mutation.Fields)
                    {
                        AddFieldToType(_mutationObject, field);
                    }
                }
            }

            return new GraphQL.Types.Schema
            {
                Query = _queryObject,
                Mutation = _mutationObject,
            };
        }

        private void EnsureObjectExists(ref ObjectGraphType obj, string name, string description)
        {
            if (obj == null)
            {
                obj = new ObjectGraphType
                {
                    Name = "Query",
                    Description = "Queries.",
                };
            }
        }

        private void AddFieldToType(ObjectGraphType type, FieldType field)
        {
            var newField = type.Field(field.Type, field.Name, field.Description, field.Arguments, field.Resolve);
            newField.DeprecationReason = field.DeprecationReason;
        }
    }
}
