using System;
using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Condition")]
    public class Condition<Agent, TSensory, TAction> : Node<Agent, TSensory, TAction>
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
        public ConditionEvaluator<Agent, TSensory> evaluator { get; private set; }

        [ConstructorParameter("childTrue")]
        public Node<Agent, TSensory, TAction> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                _children = new List<Node<Agent, TSensory, TAction>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent, TSensory, TAction> _childTrue;

        [ConstructorParameter("childFalse")]
        public Node<Agent, TSensory, TAction> childFalse
        {
            get { return _childFalse; }
            private set
            {
                _childFalse = value;
                _children = new List<Node<Agent, TSensory, TAction>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent, TSensory, TAction> _childFalse;

        public Condition(
            ConditionEvaluator<Agent, TSensory> evaluator,
            Node<Agent, TSensory, TAction> childTrue,
            Node<Agent, TSensory, TAction> childFalse
        )
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
            this.childTrue = childTrue;
            this.childFalse = childFalse;
        }

        public override TAction Tick(TSensory input)
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

            TAction result;
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

        internal new void SetBehaviorTree(BehaviorTree<Agent, TSensory, TAction> tree)
        {
            base.SetBehaviorTree(tree);

            // If the evaluator implements IBehaviorTreeAware, set its behavior tree reference
            if (evaluator is IBehaviorTreeAware<Agent, TSensory, TAction> treeAware)
            {
                treeAware.SetBehaviorTree(tree);
            }
        }

        private TAction RunNode(Node<Agent, TSensory, TAction> node, TSensory input)
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
