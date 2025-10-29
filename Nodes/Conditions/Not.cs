using System.Collections.Generic;

namespace BehaviorTree.Nodes
{
    public class Not<Agent> : Logic<Agent>
    {
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
