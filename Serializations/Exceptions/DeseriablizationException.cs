using System;

namespace BehaviorTree.Serializations
{
    public class DeserializationException : Exception
    {
        public DeserializationException(string message, string[] stacks)
            : base($"{message} (Stack: {string.Join(" -> ", stacks)})") { }
    }
}
