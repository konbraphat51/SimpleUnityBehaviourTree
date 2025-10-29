using System.Linq;

namespace BehaviorTree.Nodes
{
    public class Or<Agent> : Logic<Agent>
    {
        public Or(ConditionEvaluator<Agent>[] conditions)
            : base("Or")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(Agent agent)
        {
            foreach (ConditionEvaluator<Agent> condition in _children)
            {
                if (condition.Evaluate(agent))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
