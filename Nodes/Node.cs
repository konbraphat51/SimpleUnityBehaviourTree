using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class Node<Agent, TSensory, TAction> : ISerializableBT
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
        protected List<Node<Agent, TSensory, TAction>> _children = new List<Node<Agent, TSensory, TAction>>();
        public IReadOnlyList<Node<Agent, TSensory, TAction>> children
        {
            get { return _children.AsReadOnly(); }
        }

        // Reference to the behavior tree for tracking current action
        internal BehaviorTree<Agent, TSensory, TAction> behaviorTree { get; set; }

        public Node(string name)
        {
            this.name = name;
        }

        public abstract TAction Tick(TSensory input);

        public virtual void Reset()
        {
            foreach (Node<Agent, TSensory, TAction> child in children)
            {
                child.Reset();
            }
        }

        /// <summary>
        /// Sets the behavior tree reference for this node and all its children.
        /// </summary>
        internal void SetBehaviorTree(BehaviorTree<Agent, TSensory, TAction> tree)
        {
            behaviorTree = tree;
            foreach (Node<Agent, TSensory, TAction> child in children)
            {
                child.SetBehaviorTree(tree);
            }
        }
    }
}
