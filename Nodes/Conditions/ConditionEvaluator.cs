using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class ConditionEvaluator<Agent, TSensory> : ISerializableBT
        where TSensory : struct
    {
        public string name { get; private set; }

        public ConditionEvaluator(string name)
        {
            this.name = name;
        }

        public abstract bool Evaluate(TSensory input, BtInformation btInfo);
    }
}
