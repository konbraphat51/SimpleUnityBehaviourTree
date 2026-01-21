using BehaviorTree.Nodes;
using System;

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

        private DateTime _lastTickTime;
        private bool _isFirstTick = true;

        public BehaviorTree(string name, Node<Agent, TSensory, TAction> root, Agent agent)
        {
            this.name = name;
            nodeRoot = root;
            this.agent = agent;
        }

        public TAction Tick(TSensory input)
        {
            // Calculate deltaTime using system time
            float deltaTime = 0f;
            DateTime currentTime = DateTime.UtcNow;
            
            if (_isFirstTick)
            {
                _isFirstTick = false;
                deltaTime = 0f;
            }
            else
            {
                deltaTime = (float)(currentTime - _lastTickTime).TotalSeconds;
            }
            
            _lastTickTime = currentTime;

            BtInformation btInfo = new BtInformation(deltaTime);
            TAction result = nodeRoot.Tick(input, btInfo);
            return result;
        }
    }
}
