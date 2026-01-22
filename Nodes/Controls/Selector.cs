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

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            // try nodes from the beginning of the list in order each tick
            // execute the first one that returns a non-default result
            for (int i = 0; i < children.Count; i++)
            {
                TAction result = children[i].Tick(input, btInfo);

                // If this child returns a non-default result, use it
                if (!result.Equals(default(TAction)))
                {
                    return result;
                }
            }

            // if all children returned default or no children exist
            return default(TAction);
        }
    }
}
