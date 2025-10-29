namespace BehaviorTree.Nodes
{
    public class And<Agent> : ConditionEvaluator<Agent>
    {
        public ConditionEvaluator<Agent>[] conditions;

        public And(ConditionEvaluator<Agent>[] conditions)
            : base("And")
        {
            this.conditions = conditions;
        }

        public override bool Evaluate(Agent agent)
        {
            foreach (ConditionEvaluator<Agent> condition in conditions)
            {
                if (!condition.Evaluate(agent))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
