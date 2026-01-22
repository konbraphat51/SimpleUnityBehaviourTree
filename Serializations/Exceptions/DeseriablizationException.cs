using System;

namespace SimpleUnityBehaviorTree.Serializations
{
    public class DeserializationException : Exception
    {
        public DeserializationException(string message, string[] stacks)
            : base($"{message} (Stack: {string.Join(" -> ", stacks)})") { }
    }
}
