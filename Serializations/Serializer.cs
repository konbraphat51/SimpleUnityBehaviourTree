using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BehaviorTree.Nodes;
using Newtonsoft.Json;

namespace BehaviorTree.Serializations
{
    /// <summary>
    /// To Json
    /// </summary>
    public static class Serializer<Agent>
    {
        public static string WriteNodeJson(Node<Agent> node)
        {
            StringWriter stringWriter = new StringWriter();
            JsonWriter writer = new JsonTextWriter(stringWriter);

            // null guard
            if (node == null)
            {
                writer.WriteNull();
                return writer.ToString();
            }

            // {
            writer.WriteStartObject();

            // "type": "TypeName",
            writer.WritePropertyName("type");
            string objectName = GetTypeName(node);
            writer.WriteValue(objectName);

            // "params": { ... }
            WriteParameters(writer, node, objectName);

            // }
            writer.WriteEndObject();

            return stringWriter.ToString();
        }

        public static string WriteEvaluatorJson(ConditionEvaluator<Agent> evaluator)
        {
            StringWriter stringWriter = new StringWriter();
            JsonWriter writer = new JsonTextWriter(stringWriter);

            // null guard
            if (evaluator == null)
            {
                writer.WriteNull();
                return writer.ToString();
            }

            // {
            writer.WriteStartObject();

            // "type": "TypeName",
            writer.WritePropertyName("type");
            string objectName = GetTypeName(evaluator);
            writer.WriteValue(objectName);

            // "params": { ... }
            WriteParameters(writer, evaluator, objectName);

            // }
            writer.WriteEndObject();

            return stringWriter.ToString();
        }

        private static void WriteParameters(
            JsonWriter writer,
            ISerializableBT target,
            string objectName
        )
        {
            // "params": {
            writer.WritePropertyName("params");
            writer.WriteStartObject();

            // ... constructor parameters ...
            WriteConstructorParams(writer, target, objectName);

            // }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Read the value of `SerializableNode` attribute from the node type.
        /// </summary>
        private static string GetTypeName(Node<Agent> node)
        {
            object[] attrs = node.GetType().GetCustomAttributes(typeof(SerializableNode), false);
            if (attrs.Length == 0)
            {
                throw new Exception(
                    $"Node class {node.GetType().Name} does not have SerializableNode attribute."
                );
            }
            SerializableNode serializableNodeAttr = (SerializableNode)attrs[0];
            return serializableNodeAttr.typeName;
        }

        /// <summary>
        /// Read the value of `SerializableEvaluator` attribute from the node type.
        /// </summary>
        private static string GetTypeName(ConditionEvaluator<Agent> evaluator)
        {
            object[] attrs = evaluator
                .GetType()
                .GetCustomAttributes(typeof(SerializableEvaluator), false);
            if (attrs.Length == 0)
            {
                throw new Exception(
                    $"Evaluator class {evaluator.GetType().Name} does not have SerializableEvaluator attribute."
                );
            }
            SerializableEvaluator serializableEvaluatorAttr = (SerializableEvaluator)attrs[0];
            return serializableEvaluatorAttr.typeName;
        }

        private static void WriteConstructorParams(
            JsonWriter writer,
            ISerializableBT target,
            string objectName
        )
        {
            Tuple<string, PropertyInfo>[] parameters = GetConstructorParameters(target);

            foreach (Tuple<string, PropertyInfo> param in parameters)
            {
                // "paramName":
                writer.WritePropertyName(param.Item1);

                // :paramValue
                WriteValue(writer, param.Item2.GetValue(target), objectName);
            }
        }

        private static void WriteValue(JsonWriter writer, object value, string objectName)
        {
            // if array...
            if (value is System.Collections.IEnumerable && !(value is string))
            {
                writer.WriteStartArray();
                foreach (object item in (System.Collections.IEnumerable)value)
                {
                    WriteSingleValue(writer, item, objectName);
                }
                writer.WriteEndArray();
            }
            // ... if single value...
            else
            {
                WriteSingleValue(writer, value, objectName);
            }
        }

        private static void WriteSingleValue(JsonWriter writer, object value, string objectName)
        {
            switch (value.GetType())
            {
                case Type t when typeof(Node<Agent>).IsAssignableFrom(t):
                    string childJson = WriteNodeJson((Node<Agent>)value);
                    writer.WriteRawValue(childJson);
                    break;
                case Type t when typeof(ConditionEvaluator<Agent>).IsAssignableFrom(t):
                    string evaluationJson = WriteEvaluatorJson((ConditionEvaluator<Agent>)value);
                    writer.WriteRawValue(evaluationJson);
                    break;
                default:
                    try
                    {
                        writer.WriteValue(value);
                    }
                    catch (JsonWriterException e)
                    {
                        throw new Exception(
                            $"Failed to serialize value of type \"{value.GetType().Name}\" in Node/Evaluator \"{objectName}\": {e.Message}"
                        );
                    }
                    break;
            }
        }

        /// <summary>
        /// Get all fields with `ConstructorParameter` attribute.
        /// </summary>
        private static Tuple<string, PropertyInfo>[] GetConstructorParameters(
            ISerializableBT target
        )
        {
            List<Tuple<string, PropertyInfo>> result = new List<Tuple<string, PropertyInfo>>();

            PropertyInfo[] properties = target
                .GetType()
                .GetProperties(
                    BindingFlags.NonPublic
                        | BindingFlags.Public
                        | BindingFlags.Instance
                        | BindingFlags.DeclaredOnly
                );

            foreach (PropertyInfo prop in properties)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(ConstructorParameter), false);
                if (attrs.Length > 0)
                {
                    ConstructorParameter parameterAttr = (ConstructorParameter)attrs[0];
                    result.Add(new Tuple<string, PropertyInfo>(parameterAttr.parameterName, prop));
                }
            }
            return result.ToArray();
        }
    }
}
