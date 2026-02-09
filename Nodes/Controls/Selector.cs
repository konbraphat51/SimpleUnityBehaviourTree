using System.Linq;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    [SerializableNode("Selector")]
    public class Selector<TSensory, TAction> : Node<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        [ConstructorParameter("children")]
        public Node<TSensory, TAction>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        private int _lastSelectedNodeIndex = -1;

        public Selector(Node<TSensory, TAction>[] children)
            : base("Selector")
        {
            _children = children.ToList();
        }

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            // Try nodes from the beginning in order
            // Execute the first one that can run
            for (int cnt = 0; cnt < children.Count; cnt++)
            {
                // Check if this child can run
                if (children[cnt].CanRun(input, btInfo))
                {
                    // If the selected node is different from the last one, reset the last node
                    if (_lastSelectedNodeIndex != -1 && _lastSelectedNodeIndex != cnt)
                    {
                        children[_lastSelectedNodeIndex].Reset();
                    }

                    // Update the last selected node index
                    _lastSelectedNodeIndex = cnt;

                    // Execute this child and return its action
                    TAction action = children[cnt].Tick(input, btInfo);
                    return action;
                }
            }

            // All children failed to run
            // Reset the last selected node if there was one
            if (_lastSelectedNodeIndex != -1)
            {
                children[_lastSelectedNodeIndex].Reset();
                _lastSelectedNodeIndex = -1;
            }

            return default(TAction);
        }

        public override bool CanRun(TSensory input, BtInformation btInfo)
        {
            // Selector can always run
            return true;
        }
    }
}
