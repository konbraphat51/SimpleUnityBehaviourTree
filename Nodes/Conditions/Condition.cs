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

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            return childTrue.Tick(input, btInfo);
        }

        public override bool CanRun(TSensory input, BtInformation btInfo)
        {
            // Can run if the condition evaluates to true
            return evaluator.Evaluate(input, btInfo);
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
