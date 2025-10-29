namespace BehaviorTree.Nodes
{
    public abstract class ConditionEvaluator<Agent>
    {
        public string name { get; private set; }

        public ConditionEvaluator(string name)
        {
            this.name = name;
        }

        public abstract bool Evaluate(Agent agent);
    }
}
