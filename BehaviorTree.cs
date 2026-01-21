using BehaviorTree.Nodes;
using System;

namespace BehaviorTree
{
    public class BehaviorTree<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public string name;
        public Node<TSensory, TAction> nodeRoot;

        private DateTime _lastTickTime;
        private bool _isFirstTick = true;

        public BehaviorTree(string name, Node<TSensory, TAction> root)
        {
            this.name = name;
            nodeRoot = root;
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
