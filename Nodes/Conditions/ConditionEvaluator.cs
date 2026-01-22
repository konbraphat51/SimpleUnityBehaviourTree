using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    public abstract class ConditionEvaluator<TSensory> : ISerializableBT
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
