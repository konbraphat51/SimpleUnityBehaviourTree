using System;
using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Sequence")]
    public class Sequence<Agent, TInput, TOutput> : Node<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        public int childCurrent { get; private set; } = -1;

        [ConstructorParameter("children")]
        public Node<Agent, TInput, TOutput>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        public Sequence(Node<Agent, TInput, TOutput>[] children)
            : base("Sequence")
        {
            _children = children.ToList();
        }

        public override TOutput Tick(TInput input)
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
                return CreateSuccessOutput(input);
            }

            // tick the current child
            TOutput result = children[childCurrent].Tick(input);

            // Check the state of the result
            State resultState = GetStateFromOutput(result);

            // process by result
            switch (resultState)
            {
                case State.SUCCESS:
                {
                    // if there are more children...
                    if (childCurrent + 1 < children.Count)
                    {
                        // ... move to the next child
                        childCurrent++;
                        return CreateRunningOutput(input);
                    }
                    else
                    {
                        // ... sequence succeeded
                        childCurrent = -1;
                        Reset();
                        return result;
                    }
                }
                case State.FAILURE:
                {
                    // sequence fails immediately
                    childCurrent = -1;
                    Reset();
                    return result;
                }
                case State.RUNNING:
                {
                    return result;
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

        public void AddChild(Node<Agent, TInput, TOutput> child)
        {
            _children.Add(child);
        }

        public void RemoveChild(Node<Agent, TInput, TOutput> child)
        {
            _children.Remove(child);
        }

        // Helper methods to create output with state
        protected virtual TOutput CreateSuccessOutput(TInput input)
        {
            // Default implementation - subclasses should override
            return default(TOutput);
        }

        protected virtual TOutput CreateRunningOutput(TInput input)
        {
            // Default implementation - subclasses should override
            return default(TOutput);
        }

        protected virtual State GetStateFromOutput(TOutput output)
        {
            // Default implementation - subclasses should override
            // This assumes TOutput has a State field
            return State.SUCCESS;
        }
    }
}
