using System;
using System.Collections.Generic;

namespace BehaviorTree.Nodes
{
    public class Condition<Agent> : Node<Agent>
    {
        public enum Evaluation
        {
            TRUE,
            FALSE,
            NOT_YET,
        }

        public Evaluation currentEvaluation { get; private set; } = Evaluation.NOT_YET;
        public ConditionEvaluator<Agent> evaluator { get; private set; }
        private Node<Agent> _childTrue;
        public Node<Agent> childTrue
        {
            get { return _childTrue; }
            private set
            {
                _childTrue = value;
                children = new List<Node<Agent>> { _childTrue, _childFalse };
            }
        }
        private Node<Agent> _childFalse;
        public Node<Agent> childFalse
        {
            get { return _childFalse; }
            private set
            {
                _childFalse = value;
                children = new List<Node<Agent>> { _childTrue, _childFalse };
            }
        }

        public Condition(ConditionEvaluator<Agent> evaluator)
            : base(evaluator.name)
        {
            this.evaluator = evaluator;
        }

        public override State Tick(Agent agent)
        {
            // if not evaluated yet...
            if (currentEvaluation == Evaluation.NOT_YET)
            {
                // ...evaluate condition
                bool evaluation = evaluator.Evaluate(agent);
                currentEvaluation = evaluation ? Evaluation.TRUE : Evaluation.FALSE;
            }

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

            currentEvaluation = Evaluation.NOT_YET;
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
