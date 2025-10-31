using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorTree.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BehaviorTree.Serializations
{
    public static class Deserializer<Agent>
    {
        public static Node<Agent> ReadNodeJson(string json, string[] stacks = null)
        {
            if (stacks == null)
            {
                stacks = new string[] { "Root" };
            }

            JsonReader reader = new JsonTextReader(new System.IO.StringReader(json));
            JObject jObjectRoot = JObject.Load(reader);

            // read type name
            string typeName = ReadTypeName(jObjectRoot, stacks);
            Type nodeType = FindNodeByName(typeName, stacks);

            // read "params"
            JObject jObjectParams = ReadParamsObject(jObjectRoot, stacks);

            // create Node instance
            return (Node<Agent>)CreateInstance(nodeType, jObjectParams, stacks);
        }

        public static ConditionEvaluator<Agent> ReadEvaluatorJson(
            string json,
            string[] stacks = null
        )
        {
            if (stacks == null)
            {
                stacks = new string[] { "Root" };
            }

            JsonReader reader = new JsonTextReader(new System.IO.StringReader(json));
            JObject jObjectRoot = JObject.Load(reader);

            // read type name
            string typeName = ReadTypeName(jObjectRoot, stacks);
            Type evaluatorType = FindEvaluatorByName(typeName, stacks);

            // read "params"
            JObject jObjectParams = ReadParamsObject(jObjectRoot, stacks);

            // create Evaluator instance
            return (ConditionEvaluator<Agent>)CreateInstance(evaluatorType, jObjectParams, stacks);
        }

        private static string ReadTypeName(JObject jObject, string[] stacks)
        {
            if (jObject.TryGetValue("type", out JToken typeToken))
            {
                return typeToken.ToObject<string>();
            }
            else
            {
                throw new DeserializationException($"Missing 'type' field", stacks);
            }
        }

        private static JObject ReadParamsObject(JObject jObject, string[] stacks)
        {
            if (jObject.TryGetValue("params", out JToken paramsToken))
            {
                return paramsToken.ToObject<JObject>();
            }
            else
            {
                throw new DeserializationException($"Missing 'params' field", stacks);
            }
        }

        private static ISerializableBT CreateInstance(
            Type targetType,
            JObject jObjectParams,
            string[] stacks
        )
        {
            // get constructor parameters
            ConstructorInfo constructor = targetType.GetConstructors().FirstOrDefault();
            ParameterInfo[] paramInfos = constructor.GetParameters();

            // read corresponding values from json
            object[] paramValues = new object[paramInfos.Length];
            for (int cnt = 0; cnt < paramInfos.Length; cnt++)
            {
                ParameterInfo paramInfo = paramInfos[cnt];
                string paramName = paramInfo.Name;

                if (jObjectParams.TryGetValue(paramName, out JToken paramToken))
                {
                    object paramValue = ConvertJTokenToParameter(
                        paramToken,
                        paramInfo.ParameterType,
                        stacks
                    );
                    paramValues[cnt] = paramValue;
                }
                else
                {
                    throw new DeserializationException(
                        $"Missing constructor parameter '{paramName}' for Node '{targetType.Name}'",
                        stacks
                    );
                }
            }

            return (ISerializableBT)constructor.Invoke(paramValues);
        }

        private static object ConvertJTokenToParameter(
            JToken token,
            Type targetType,
            string[] stacks
        )
        {
            if (targetType.IsArray)
            {
                // array type
                Type elementType = targetType.GetElementType();
                JArray jArray = token.ToObject<JArray>();
                Array outputArray = Array.CreateInstance(elementType, jArray.Count);
                for (int cnt = 0; cnt < jArray.Count; cnt++)
                {
                    object elementValue = ConvertJTokenToParameter(
                        jArray[cnt],
                        elementType,
                        stacks
                    );
                    outputArray.SetValue(elementValue, cnt);
                }
                return outputArray;
            }
            else if (typeof(Node<Agent>).IsAssignableFrom(targetType))
            {
                // nested Node
                string childJson = token.ToString();
                return ReadNodeJson(childJson, stacks);
            }
            else if (typeof(ConditionEvaluator<Agent>).IsAssignableFrom(targetType))
            {
                // nested Evaluator
                string evaluatorJson = token.ToString();
                return ReadEvaluatorJson(evaluatorJson, stacks);
            }
            else
            {
                // primitive type
                return token.ToObject(targetType);
            }
        }

        /// <summary>
        /// Find Node class has SerializableNode attribute matching the type name.
        /// </summary>
        private static Type FindNodeByName(string typeName, string[] stacks)
        {
            return FindSerializableByName(typeof(SerializableNode), typeName, stacks);
        }

        /// <summary>
        /// Find Evaluator class has SerializableEvaluator attribute matching the type name.
        /// </summary>
        private static Type FindEvaluatorByName(string typeName, string[] stacks)
        {
            return FindSerializableByName(typeof(SerializableEvaluator), typeName, stacks);
        }

        private static Type FindSerializableByName(
            Type typeAttribute,
            string typeName,
            string[] stacks
        )
        {
            IEnumerable<Type> allTypes = AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());

            // find types has targeted attribute
            Type targetType = allTypes.FirstOrDefault(type =>
                type.GetCustomAttributes(typeAttribute, false)
                    .Cast<ISerializableAttribute>()
                    .Any(attr => attr.typeName == typeName)
            );

            // null guard
            if (targetType == null)
            {
                throw new DeserializationException(
                    $"Cannot find class with attribute '{typeAttribute.Name}' whose name is '{typeName}'.",
                    stacks
                );
            }

            // if non-generic = Agent type defined in inheritance...
            if (!targetType.IsGenericTypeDefinition)
            {
                // ... directly return
                return targetType;
            }
            // if generic = Agent typing required...
            else
            {
                // ... make generic type with Agent
                return targetType.MakeGenericType(typeof(Agent));
            }
        }
    }
}
