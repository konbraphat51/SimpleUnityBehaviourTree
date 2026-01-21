using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Selector")]
    public class Selector<Agent, TSensory, TAction> : Node<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        [ConstructorParameter("children")]
        public Node<Agent, TSensory, TAction>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        public Selector(Node<Agent, TSensory, TAction>[] children)
            : base("Selector")
        {
            _children = children.ToList();
        }

        public override TAction Tick(TSensory input)
        {
            // try nodes from the beginning of the list in order each tick
            // execute the first one that returns a non-default result
            for (int i = 0; i < children.Count; i++)
            {
                TAction result = children[i].Tick(input);
                
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
