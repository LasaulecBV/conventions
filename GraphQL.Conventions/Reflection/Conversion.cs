using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQL.Conventions.Reflection
{
    static class Conversion
    {
        private static Dictionary<Tuple<Type, Type>, MethodInfo> _conversionMethods =
            new Dictionary<Tuple<Type, Type>, MethodInfo>();

        public static object ConvertImplicity(object value, Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return value;
            }

            MethodInfo implicitConversionMethod;
            var key = Tuple.Create(fromType, toType);
            
            if (!_conversionMethods.TryGetValue(key, out implicitConversionMethod))
            {
                implicitConversionMethod = GetImplicitConversionMethod(fromType, toType);
                _conversionMethods[key] = implicitConversionMethod;
            }

            try
            {
                return implicitConversionMethod?.Invoke(null, new[] { value });
            }
            catch (Exception ex)
            {
                throw ex?.InnerException ?? ex;
            }
        }

        private static MethodInfo GetImplicitConversionMethod(Type fromType, Type toType)
        {
            var methodInfo = GetImplicitConversionMethod(fromType, fromType, toType);
            if (methodInfo != null)
            {
                return methodInfo;
            }

            methodInfo = GetImplicitConversionMethod(toType, fromType, toType);
            if (methodInfo != null)
            {
                return methodInfo;
            }

            return null;
        }

        private static MethodInfo GetImplicitConversionMethod(Type baseType, Type fromType, Type toType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var methodInfo = GetImplicitConversionMethod(baseType.GenericTypeArguments.First(), fromType, toType);
                if (methodInfo != null)
                {
                    return methodInfo;
                }
            }

            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == toType)
                .FirstOrDefault(mi =>
                {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == fromType;
                });
        }
    }
}
