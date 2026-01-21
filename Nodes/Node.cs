using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class Node<TSensory, TAction> : ISerializableBT
        where TSensory : struct
        where TAction : struct
    {
        public enum State
        {
            RUNNING,
            SUCCESS,
            FAILURE,
        }

        public string name { get; private set; }
        protected List<Node<TSensory, TAction>> _children = new List<Node<TSensory, TAction>>();
        public IReadOnlyList<Node<TSensory, TAction>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Node(string name)
        {
            this.name = name;
        }

        public abstract TAction Tick(TSensory input, BtInformation btInfo);

        public virtual void Reset()
        {
            foreach (Node<TSensory, TAction> child in children)
            {
                child.Reset();
            }
        }
    }
}
