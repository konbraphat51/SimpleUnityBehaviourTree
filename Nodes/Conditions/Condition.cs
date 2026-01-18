using System;
using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Condition")]
    public class Condition<Agent> : Node<Agent>
    {
        public enum Evaluation
        {
            TRUE,
            FALSE,
        }

        public Evaluation currentEvaluation { get; private set; } = Evaluation.FALSE;

        [ConstructorParameter("evaluator")]
        public ConditionEvaluator<Agent> evaluator { get; private set; }

        [ConstructorParameter("childTrue")]
        public Node<Agent> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                _children = new List<Node<Agent>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent> _childTrue;

        [ConstructorParameter("childFalse")]
        public Node<Agent> childFalse
        {
            get { return _childFalse; }
            private set
            {
                _childFalse = value;
                _children = new List<Node<Agent>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent> _childFalse;

        public Condition(
            ConditionEvaluator<Agent> evaluator,
            Node<Agent> childTrue,
            Node<Agent> childFalse
        )
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
            this.childTrue = childTrue;
            this.childFalse = childFalse;
        }

        public override State Tick(Agent agent)
        {
            // Re-evaluate condition every frame
            bool evaluation = evaluator.Evaluate(agent);
            Evaluation newEvaluation = evaluation ? Evaluation.TRUE : Evaluation.FALSE;

            // If evaluation changed, reset the previous child
            if (currentEvaluation != newEvaluation)
            {
                if (currentEvaluation == Evaluation.TRUE && childTrue != null)
                {
                    childTrue.Reset();
                }
                else if (currentEvaluation == Evaluation.FALSE && childFalse != null)
                {
                    childFalse.Reset();
                }
            }

            currentEvaluation = newEvaluation;

            State result;
            switch (currentEvaluation)
            {
                case Evaluation.TRUE:
                    result = RunNode(childTrue, agent);
                    break;
                case Evaluation.FALSE:
                    result = RunNode(childFalse, agent);
                    break;
                default:
                    throw new NotImplementedException(
                        "Condition evaluation state not implemented."
                    );
            }

            // if child finished...
            if (result != State.RUNNING)
            {
                // reset evaluation for next tick
                Reset();
            }

            return result;
        }

        public override void Reset()
        {
            base.Reset();

            currentEvaluation = Evaluation.FALSE;
        }

        private State RunNode(Node<Agent> node, Agent agent)
        {
            if (node != null)
            {
                return node.Tick(agent);
            }
            else
            {
                // null guard
                return State.FAILURE;
            }
        }
    }
}
