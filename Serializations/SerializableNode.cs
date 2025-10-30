using System;

namespace BehaviorTree.Serializations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializableNode : Attribute
    {
        public string nodeTypeName { get; private set; }

        public SerializableNode(string nodeTypeName)
        {
            this.nodeTypeName = nodeTypeName;
        }
    }
}
