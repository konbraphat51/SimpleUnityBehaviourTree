using System;

namespace BehaviorTree.Serializations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializableNode : Attribute, ISerializableAttribute
    {
        public string typeName { get; private set; }

        public SerializableNode(string nodeTypeName)
        {
            this.typeName = nodeTypeName;
        }
    }
}
