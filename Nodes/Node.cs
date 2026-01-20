using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class Node<Agent, TInput, TOutput> : ISerializableBT
        where TInput : struct
        where TOutput : struct
    {
        public enum State
        {
            RUNNING,
            SUCCESS,
            FAILURE,
        }

        public string name { get; private set; }
        protected List<Node<Agent, TInput, TOutput>> _children = new List<Node<Agent, TInput, TOutput>>();
        public IReadOnlyList<Node<Agent, TInput, TOutput>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Node(string name)
        {
            this.name = name;
        }

        public abstract TOutput Tick(TInput input);

        public virtual void Reset()
        {
            foreach (Node<Agent, TInput, TOutput> child in children)
            {
                child.Reset();
            }
        }
    }
}
