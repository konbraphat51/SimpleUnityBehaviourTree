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
        public enum Evaluation
        {
            TRUE,
            FALSE,
            NOT_YET,
        }

        public Evaluation currentEvaluation { get; private set; } = Evaluation.NOT_YET;

        [ConstructorParameter("evaluator")]
        public ConditionEvaluator<TSensory> evaluator { get; private set; }

        [ConstructorParameter("childTrue")]
        public Node<TSensory, TAction> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                _children = new List<Node<TSensory, TAction>> { _childTrue, _childFalse };
            }
        }
        private Node<TSensory, TAction> _childTrue;

        [ConstructorParameter("childFalse")]
        public Node<TSensory, TAction> childFalse
        {
            get { return _childFalse; }
            private set
            {
                _childFalse = value;
                _children = new List<Node<TSensory, TAction>> { _childTrue, _childFalse };
            }
        }
        private Node<TSensory, TAction> _childFalse;

        public Condition(
            ConditionEvaluator<TSensory> evaluator,
            Node<TSensory, TAction> childTrue,
            Node<TSensory, TAction> childFalse
        )
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
            this.childTrue = childTrue;
            this.childFalse = childFalse;
        }

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            // Re-evaluate condition every frame
            bool evaluation = evaluator.Evaluate(input, btInfo);
            Evaluation newEvaluation = evaluation ? Evaluation.TRUE : Evaluation.FALSE;

            // If evaluation changed, reset all children
            if (currentEvaluation != Evaluation.NOT_YET && currentEvaluation != newEvaluation)
            {
                if (childTrue != null)
                {
                    childTrue.Reset();
                }
                if (childFalse != null)
                {
                    childFalse.Reset();
                }
            }

            currentEvaluation = newEvaluation;

            TAction result;
            switch (currentEvaluation)
            {
                case Evaluation.TRUE:
                    result = RunNode(childTrue, input, btInfo);
                    break;
                case Evaluation.FALSE:
                    result = RunNode(childFalse, input, btInfo);
                    break;
                default:
                    throw new NotImplementedException(
                        "Condition evaluation state not implemented."
                    );
            }

            // Check the state of the result
            State resultState = GetStateFromOutput(result);

            // if child finished...
            if (resultState != State.RUNNING)
            {
                // reset evaluation for next tick
                Reset();
            }

            return result;
        }

        public override void Reset()
        {
            base.Reset();

            currentEvaluation = Evaluation.NOT_YET;
        }

        private TAction RunNode(Node<TSensory, TAction> node, TSensory input, BtInformation btInfo)
        {
            if (node != null)
            {
                return node.Tick(input, btInfo);
            }
            else
            {
                // null guard
                return CreateFailureOutput(input);
            }
        }

        // Helper methods to create output with state
        protected virtual TAction CreateFailureOutput(TSensory input)
        {
            // Default implementation - subclasses should override
            return default(TAction);
        }

        protected virtual State GetStateFromOutput(TAction output)
        {
            // Default implementation - subclasses should override
            // This assumes TAction has a State field
            return State.SUCCESS;
        }
    }
}
