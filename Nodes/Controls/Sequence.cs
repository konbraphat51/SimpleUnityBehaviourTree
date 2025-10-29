using System;
using System.Linq;

namespace BehaviorTree.Nodes
{
    public class Sequence<Agent> : Node<Agent>
    {
        public int childCurrent { get; private set; } = -1;

        public Sequence(string name, Node<Agent>[] children)
            : base(name)
        {
            _children = children.ToList();
        }

        public override State Tick(Agent agent)
        {
            // if starting sequence...
            if (childCurrent == -1 && children.Count > 0)
            {
                // ... start from the first child
                childCurrent = 0;
            }
            // if no child...
            else if (children.Count == 0)
            {
                // ... just return success
                return State.SUCCESS;
            }

            // tick the current child
            State result = children[childCurrent].Tick(agent);

            // process by result
            switch (result)
            {
                case State.SUCCESS:
                {
                    // if there are more children...
                    if (childCurrent + 1 < children.Count)
                    {
                        // ... move to the next child
                        childCurrent++;
                        return State.RUNNING;
                    }
                    else
                    {
                        // ... sequence succeeded
                        childCurrent = -1;
                        Reset();
                        return State.SUCCESS;
                    }
                }
                case State.FAILURE:
                {
                    // sequence fails immediately
                    childCurrent = -1;
                    Reset();
                    return State.FAILURE;
                }
                case State.RUNNING:
                {
                    return State.RUNNING;
                }
                default:
                {
                    throw new NotImplementedException("Unhandled state in NodeSequence");
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            childCurrent = -1;
        }

        public void AddChild(Node<Agent> child)
        {
            _children.Add(child);
        }

        public void RemoveChild(Node<Agent> child)
        {
            _children.Remove(child);
        }
    }
}
