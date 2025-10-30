using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class ConditionEvaluator<Agent> : ISerializableBT
    {
        public string name { get; private set; }

        public ConditionEvaluator(string name)
        {
            this.name = name;
        }

        public abstract bool Evaluate(Agent agent);
    }
}
