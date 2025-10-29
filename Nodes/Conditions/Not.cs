namespace BehaviorTree.Nodes
{
    public class Not<Agent> : ConditionEvaluator<Agent>
    {
        public ConditionEvaluator<Agent> condition;

        public Not(ConditionEvaluator<Agent> condition)
            : base("Not")
        {
            this.condition = condition;
        }

        public override bool Evaluate(Agent agent)
        {
            return !condition.Evaluate(agent);
        }
    }
}
