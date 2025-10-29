namespace BehaviorTree.Nodes
{
    public class Or<Agent> : ConditionEvaluator<Agent>
    {
        public ConditionEvaluator<Agent>[] conditions;

        public Or(ConditionEvaluator<Agent>[] conditions)
            : base("Or")
        {
            this.conditions = conditions;
        }

        public override bool Evaluate(Agent agent)
        {
            foreach (ConditionEvaluator<Agent> condition in conditions)
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
