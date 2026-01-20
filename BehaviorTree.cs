using BehaviorTree.Nodes;

namespace BehaviorTree
{
    public class BehaviorTree<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        public string name;
        public Node<Agent, TInput, TOutput> nodeRoot;

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

        public BehaviorTree(string name, Node<Agent, TInput, TOutput> root, Agent agent)
        {
            this.name = name;
            nodeRoot = root;
            this.agent = agent;
        }

        public TOutput Tick(TInput input)
        {
            TOutput result = nodeRoot.Tick(input);
            return result;
        }
    }
}
