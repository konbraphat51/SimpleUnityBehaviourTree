using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class Node<Agent> : ISerializableBT
    {
        public enum State
        {
            RUNNING,
            SUCCESS,
            FAILURE,
        }

        public string name { get; private set; }
        protected List<Node<Agent>> _children = new List<Node<Agent>>();
        public IReadOnlyList<Node<Agent>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Node(string name)
        {
            this.name = name;
        }

        public abstract State Tick(Agent agent);

        public virtual void Reset()
        {
            foreach (Node<Agent> child in children)
            {
                child.Reset();
            }
        }
    }
}
