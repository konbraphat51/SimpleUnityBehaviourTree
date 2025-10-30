using System;

namespace BehaviorTree.Serializations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializableEvaluator : Attribute
    {
        public string evaluatorTypeName { get; private set; }

        public SerializableEvaluator(string evaluatorTypeName)
        {
            this.evaluatorTypeName = evaluatorTypeName;
        }
    }
}
