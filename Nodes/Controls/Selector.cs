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
            // execute the first child that can run
            if (children.Count > 0)
            {
                return children[0].Tick(input);
            }

            // if no children, return default
            return default(TAction);
        }
    }
}
