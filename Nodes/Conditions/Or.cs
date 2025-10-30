using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Or")]
    public class Or<Agent> : Logic<Agent>
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent>[] conditioonsArray
        {
            get { return _children.ToArray(); }
        }

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
