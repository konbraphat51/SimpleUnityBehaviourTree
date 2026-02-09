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

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            // if no children, return default
            if (children.Count == 0)
            {
                return default(TAction);
            }

            // Execute current node
            TAction currentAction = children[childCurrent].Tick(input, btInfo);
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

            // If next node can run and we've executed current at least once, move to next
            if (children[nextIndex].CanRun(input, btInfo) && hasExecutedCurrentOnce)
            {
                childCurrent = nextIndex;
                hasExecutedCurrentOnce = false;
            }

            // Return current node's action
            return currentAction;
        }

        public override bool CanRun(TSensory input, BtInformation btInfo)
        {
            // Sequence can always run
            return true;
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
