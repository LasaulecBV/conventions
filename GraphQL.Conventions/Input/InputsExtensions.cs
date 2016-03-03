using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQL.Conventions.Input
{
    public static class InputsExtensions
    {
        public static Inputs ToInputs(this string json)
        {
            var dictionary = json.ToDictionary();
            return dictionary.Any()
                ? new Inputs(dictionary)
                : new Inputs();
        }

        private static Dictionary<string, object> ToDictionary(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, object>();
            }
            var values = JsonConvert.DeserializeObject(json);
            return GetValue(values) as Dictionary<string, object>;
        }

        private static object GetValue(object value)
        {
            var objectValue = value as JObject;
            if (objectValue != null)
            {
                var output = new Dictionary<string, object>();
                foreach (var kvp in objectValue)
                {
                    output.Add(kvp.Key, GetValue(kvp.Value));
                }
                return output;
            }

            var propertyValue = value as JProperty;
            if (propertyValue != null)
            {
                return new Dictionary<string, object>
                {
                    { propertyValue.Name, GetValue(propertyValue.Value) }
                };
            }

            var arrayValue = value as JArray;
            if (arrayValue != null)
            {
                return arrayValue.Children().Aggregate(new List<object>(), (list, token) =>
                {
                    list.Add(GetValue(token));
                    return list;
                });
            }

            var rawValue = value as JValue;
            if (rawValue != null)
            {
                return rawValue.Value;
            }

            return value;
        }
    }
}
