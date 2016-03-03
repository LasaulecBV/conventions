using System.Threading.Tasks;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL.Utilities;

namespace GraphQL.Conventions.Extensions
{
    public static class SchemaExtensions
    {
        public static async Task<string> Execute(
            this ISchema schema,
            string query,
            object rootObject = null,
            Inputs inputs = null,
            string operationName = null)
        {
            var executer = new DocumentExecuter();
            var writer = new DocumentWriter();
            var result = await executer.ExecuteAsync(schema, rootObject, query, operationName, inputs);
            return writer.Write(result);
        }

        public static string GetSchemaDefinition(this ISchema schema)
        {
            var schemaPrinter = new SchemaPrinter(schema);
            return schemaPrinter.Print();
        }
    }
}
