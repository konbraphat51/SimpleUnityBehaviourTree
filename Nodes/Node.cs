using System.Collections.Generic;

namespace BehaviorTree.Nodes
{
    public enum State
    {
        RUNNING,
        SUCCESS,
        FAILURE,
    }

    public abstract class Node<Agent>
    {
        public string name { get; private set; }
        public List<Node<Agent>> children = new List<Node<Agent>>();

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
