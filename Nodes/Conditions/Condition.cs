using System;
using System.Collections.Generic;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    [SerializableNode("Condition")]
    public class Condition<TSensory, TAction> : Node<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        [ConstructorParameter("evaluator")]
        public ConditionEvaluator<TSensory> evaluator { get; private set; }

        [ConstructorParameter("childTrue")]
        public Node<TSensory, TAction> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                _children = new List<Node<TSensory, TAction>> { _childTrue };
            }
        }
        private Node<TSensory, TAction> _childTrue;

        public Condition(ConditionEvaluator<TSensory> evaluator, Node<TSensory, TAction> childTrue)
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
            this.childTrue = childTrue;
        }

        public override (bool, TAction) Tick(TSensory input, BtInformation btInfo)
        {
            // Evaluate condition
            bool evaluation = evaluator.Evaluate(input, btInfo);

            if (evaluation)
            {
                // Condition true: execute child
                return childTrue.Tick(input, btInfo);
            }
            else
            {
                // Condition false: return false with default action
                return (false, default(TAction));
            }
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
