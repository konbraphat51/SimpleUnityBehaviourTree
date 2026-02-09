using System;
using System.Linq;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    [SerializableNode("Sequence")]
    public class Sequence<TSensory, TAction> : Node<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public int childCurrent { get; private set; } = 0;
        private bool hasExecutedCurrentOnce = false;

        [ConstructorParameter("children")]
        public Node<TSensory, TAction>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        [ConstructorParameter("repeat")]
        public bool repeat { get; private set; }

        public Sequence(Node<TSensory, TAction>[] children, bool repeat = false)
            : base("Sequence")
        {
            _children = children.ToList();
            this.repeat = repeat;
        }

        public override (bool, TAction) Tick(TSensory input, BtInformation btInfo)
        {
            // if no children, return success
            if (children.Count == 0)
            {
                return (true, default(TAction));
            }

            // Execute current node
            var (currentSuccess, currentAction) = children[childCurrent].Tick(input, btInfo);
            hasExecutedCurrentOnce = true;

            // Check if we should move to next node
            int nextIndex;
            if (repeat)
            {
                // Loop back to first node after last one
                nextIndex = (childCurrent + 1) % children.Count;
            }
            else
            {
                // Repeat the last node if at the end
                nextIndex = Math.Min(childCurrent + 1, children.Count - 1);
            }
            var (nextSuccess, _) = children[nextIndex].Tick(input, btInfo);

            // If next node is true and we've executed current at least once, move to next
            if (nextSuccess && hasExecutedCurrentOnce)
            {
                childCurrent = nextIndex;
                hasExecutedCurrentOnce = false;
            }

            // Return current node's action
            return (currentSuccess, currentAction);
        }

        public override void Reset()
        {
            base.Reset();
            childCurrent = 0;
            hasExecutedCurrentOnce = false;
        }

        public void AddChild(Node<TSensory, TAction> child)
        {
            _children.Add(child);
        }

        public void RemoveChild(Node<TSensory, TAction> child)
        {
            _children.Remove(child);
        }
    }
}
