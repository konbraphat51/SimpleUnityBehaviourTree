using System.Collections.Generic;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    public abstract class Node<TSensory, TAction> : ISerializableBT
        where TSensory : struct
        where TAction : struct
    {
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

        // Returns (bool success, TAction action)
        // success: true if action was executed (condition was met)
        public abstract (bool, TAction) Tick(TSensory input, BtInformation btInfo);
        public abstract bool CanRun(TSensory input, BtInformation btInfo);

        public virtual void Reset()
        {
            foreach (Node<TSensory, TAction> child in children)
            {
                child.Reset();
            }
        }
    }
}
