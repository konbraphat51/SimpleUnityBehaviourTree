using System;

namespace SimpleUnityBehaviorTree.Serializations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializableEvaluator : Attribute, ISerializableAttribute
    {
        public string typeName { get; private set; }

        public SerializableEvaluator(string evaluatorTypeName)
        {
            this.typeName = evaluatorTypeName;
        }
    }
}
