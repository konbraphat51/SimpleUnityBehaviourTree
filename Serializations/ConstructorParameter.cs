using System;

namespace BehaviorTree.Serializations
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConstructorParameter : Attribute
    {
        public string parameterName { get; private set; }

        public ConstructorParameter(string parameterName)
        {
            this.parameterName = parameterName;
        }
    }
}
