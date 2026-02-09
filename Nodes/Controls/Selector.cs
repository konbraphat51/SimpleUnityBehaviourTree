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

        public override (bool, TAction) Tick(TSensory input, BtInformation btInfo)
        {
            // Try nodes from the beginning in order
            // Execute the first one that returns true
            for (int i = 0; i < children.Count; i++)
            {
                var (success, action) = children[i].Tick(input, btInfo);

                // If this child succeeds, return its result
                if (success)
                {
                    // If the selected node is different from the last one, reset the last node
                    if (_lastSelectedNodeIndex != -1 && _lastSelectedNodeIndex != i)
                    {
                        children[_lastSelectedNodeIndex].Reset();
                    }

                    // Update the last selected node index
                    _lastSelectedNodeIndex = i;

                    return (true, action);
                }
            }

            // All children failed
            // Reset the last selected node if there was one
            if (_lastSelectedNodeIndex != -1)
            {
                children[_lastSelectedNodeIndex].Reset();
                _lastSelectedNodeIndex = -1;
            }

            return (false, default(TAction));
        }

        public override bool CanRun(TSensory input, BtInformation btInfo)
        {
            // Selector can always run
            return true;
        }
    }
}
