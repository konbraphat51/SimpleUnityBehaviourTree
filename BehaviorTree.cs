using BehaviorTree.Nodes;

namespace BehaviorTree
{
    public class BehaviorTree<Agent>
    {
        public string name;
        public Node<Agent> nodeRoot;

        private Agent _agent;
        public Agent agent
        {
            get { return _agent; }
            set
            {
                _agent = value;
                nodeRoot.Reset();
            }
        }

        public BehaviorTree(string name, Node<Agent> root, Agent agent)
        {
            this.name = name;
            nodeRoot = root;
            this.agent = agent;
        }

        public bool Tick()
        {
            Node<Agent>.State result = nodeRoot.Tick(agent);

            // if all tree failed...
            if (result == Node<Agent>.State.FAILURE)
            {
                // ... tree execution is failing
                return false;
            }

            // if finished...
            if (result == Node<Agent>.State.SUCCESS)
            {
                // ... reset the tree
                nodeRoot.Reset();
            }

            // tree is running properly
            return true;
        }
    }
}
