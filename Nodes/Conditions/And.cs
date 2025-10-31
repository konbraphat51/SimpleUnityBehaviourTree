using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("And")]
    public class And<Agent> : Logic<Agent>
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent>[] conditionsArray
        {
            get { return _children.ToArray(); }
        }

        public And(ConditionEvaluator<Agent>[] conditions)
            : base("And")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(Agent agent)
        {
            foreach (ConditionEvaluator<Agent> condition in _children)
            {
                if (!condition.Evaluate(agent))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddCondition(ConditionEvaluator<Agent> condition)
        {
            _children.Add(condition);
        }
    }
}
