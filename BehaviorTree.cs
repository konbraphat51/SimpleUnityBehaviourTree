using BehaviorTree.Nodes;

namespace BehaviorTree
{
    public class BehaviorTree<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public string name;
        public Node<Agent, TSensory, TAction> nodeRoot;

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

        public BehaviorTree(string name, Node<Agent, TSensory, TAction> root, Agent agent)
        {
            this.name = name;
            nodeRoot = root;
            this.agent = agent;
        }

        public TAction Tick(TSensory input, float deltaTime)
        {
            BtInformation btInfo = new BtInformation(deltaTime);
            TAction result = nodeRoot.Tick(input, ref btInfo);
            return result;
        }
    }
}
