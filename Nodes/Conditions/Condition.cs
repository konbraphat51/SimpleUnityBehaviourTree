using System;
using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Condition")]
    public class Condition<Agent, TInput, TOutput> : Node<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        public enum Evaluation
        {
            TRUE,
            FALSE,
            NOT_YET,
        }

        public Evaluation currentEvaluation { get; private set; } = Evaluation.NOT_YET;

        [ConstructorParameter("evaluator")]
        public ConditionEvaluator<Agent, TInput> evaluator { get; private set; }

        [ConstructorParameter("childTrue")]
        public Node<Agent, TInput, TOutput> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                _children = new List<Node<Agent, TInput, TOutput>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent, TInput, TOutput> _childTrue;

        [ConstructorParameter("childFalse")]
        public Node<Agent, TInput, TOutput> childFalse
        {
            get { return _childFalse; }
            private set
            {
                _childFalse = value;
                _children = new List<Node<Agent, TInput, TOutput>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent, TInput, TOutput> _childFalse;

        public Condition(
            ConditionEvaluator<Agent, TInput> evaluator,
            Node<Agent, TInput, TOutput> childTrue,
            Node<Agent, TInput, TOutput> childFalse
        )
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
            this.childTrue = childTrue;
            this.childFalse = childFalse;
        }

        public override TOutput Tick(TInput input)
        {
            // Re-evaluate condition every frame
            bool evaluation = evaluator.Evaluate(input);
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

            TOutput result;
            switch (currentEvaluation)
            {
                case Evaluation.TRUE:
                    result = RunNode(childTrue, input);
                    break;
                case Evaluation.FALSE:
                    result = RunNode(childFalse, input);
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

        private TOutput RunNode(Node<Agent, TInput, TOutput> node, TInput input)
        {
            if (node != null)
            {
                return node.Tick(input);
            }
            else
            {
                // null guard
                return CreateFailureOutput(input);
            }
        }

        // Helper methods to create output with state
        protected virtual TOutput CreateFailureOutput(TInput input)
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
