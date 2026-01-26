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
                    return (true, action);
                }
            }

            // All children failed
            return (false, default(TAction));
        }
    }
}
