using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Not")]
    public class Not<Agent> : Logic<Agent>
    {
        [ConstructorParameter("condition")]
        public ConditionEvaluator<Agent> condition
        {
            get { return _children[0]; }
        }

        public Not(ConditionEvaluator<Agent> condition)
            : base("Not")
        {
            _children = new List<ConditionEvaluator<Agent>> { condition };
        }

        public override bool Evaluate(Agent agent)
        {
            return !_children[0].Evaluate(agent);
        }
    }
}
