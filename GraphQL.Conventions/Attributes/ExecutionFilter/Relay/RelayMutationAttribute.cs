using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Attributes.Relay
{
    class RelayMutationAttribute : ExecutionFilterAttributeBase
    {
        private static string InputObjectArgumentName = NameAttribute.AsParameterName(nameof(RelayMutationObject.Input));
        private static string ClientMutationIdPropertyName = nameof(IRelayMutationObject<object>.ClientMutationId);
        private static string ClientMutationIdFieldName = NameAttribute.AsFieldName(nameof(IRelayMutationObject<object>.ClientMutationId));

        internal static bool IsPotentialMutation(ICustomAttributeProvider obj)
        {
            var methodInfo = obj as MethodInfo;
            if (methodInfo == null)
            {
                return false;
            }

            var parameters = methodInfo.GetParameters();
            return parameters.Length == 1 &&
                parameters.First().Name == NameAttribute.AsParameterName(nameof(RelayMutationObject.Input));
        }

        public override void AfterExecution(ExecutionContext context)
        {
            var outputFieldDefinition = context.Entity.Fields.FirstOrDefault(field => field.Name == ClientMutationIdFieldName);
            var clientMutationIdArgumentDefinition = context.Entity.Arguments.FirstOrDefault(argument => argument.Name == ClientMutationIdFieldName);
            var inputArgumentDefinition = context.Entity.Arguments.FirstOrDefault(argument => argument.Name == InputObjectArgumentName);

            if ((clientMutationIdArgumentDefinition != null || inputArgumentDefinition != null) && outputFieldDefinition != null)
            {
                var outputField = context.Entity.TypeRepresentation.GetProperty(ClientMutationIdPropertyName);
                var inputObject = context.FieldResolutionContext.Argument<Dictionary<string, object>>(InputObjectArgumentName);
                var clientMutationId = clientMutationIdArgumentDefinition != null
                    ? context.FieldResolutionContext.Argument<string>(ClientMutationIdFieldName)
                    : null;
                object clientMutationIdBoxed;

                if (!string.IsNullOrWhiteSpace(clientMutationId))
                {
                    outputField.SetValue(context.Result, clientMutationId);
                }
                else if (inputObject != null && inputObject.TryGetValue(ClientMutationIdFieldName, out clientMutationIdBoxed))
                {
                    clientMutationId = clientMutationIdBoxed as string;
                    if (!string.IsNullOrWhiteSpace(clientMutationId))
                    {
                        outputField.SetValue(context.Result, clientMutationId);
                    }
                }
            }
        }
    }
}
