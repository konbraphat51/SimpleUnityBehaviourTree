using System;
using System.Collections.Generic;
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
            JsonWriter writer = new JsonTextWriter(new System.IO.StringWriter());

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
            writer.WriteValue(GetTypeName(node));

            // "params": { ... }
            WriteParameters(writer, node);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }

        public static string WriteEvaluatorJson(ConditionEvaluator<Agent> evaluator)
        {
            JsonWriter writer = new JsonTextWriter(new System.IO.StringWriter());

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
            writer.WriteValue(GetTypeName(evaluator));

            // "params": { ... }
            WriteParameters(writer, evaluator);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }

        private static void WriteParameters(JsonWriter writer, ISerializableBT target)
        {
            // "params": {
            writer.WritePropertyName("params");
            writer.WriteStartObject();

            // ... constructor parameters ...
            WriteConstructorParams(writer, target);

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
            return serializableNodeAttr.nodeTypeName;
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
            return serializableEvaluatorAttr.evaluatorTypeName;
        }

        private static void WriteConstructorParams(JsonWriter writer, ISerializableBT target)
        {
            Tuple<string, PropertyInfo>[] parameters = GetConstructorParameters(target);

            foreach (Tuple<string, PropertyInfo> param in parameters)
            {
                // "paramName":
                writer.WritePropertyName(param.Item1);

                // :paramValue
                WriteValue(writer, param.Item2.GetValue(target));
            }
        }

        private static void WriteValue(JsonWriter writer, object value)
        {
            // if array...
            if (value is System.Collections.IEnumerable && !(value is string))
            {
                writer.WriteStartArray();
                foreach (object item in (System.Collections.IEnumerable)value)
                {
                    WriteSingleValue(writer, item);
                }
                writer.WriteEndArray();
            }
            // ... if single value...
            else
            {
                WriteSingleValue(writer, value);
            }
        }

        private static void WriteSingleValue(JsonWriter writer, object value)
        {
            switch (value.GetType())
            {
                case Type t when t == typeof(Node<Agent>):
                    string childJson = WriteNodeJson((Node<Agent>)value);
                    writer.WriteRawValue(childJson);
                    break;
                case Type t when t == typeof(ConditionEvaluator<Agent>):
                    string evaluationJson = WriteEvaluatorJson((ConditionEvaluator<Agent>)value);
                    writer.WriteRawValue(evaluationJson);
                    break;
                default:
                    writer.WriteValue(value);
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
