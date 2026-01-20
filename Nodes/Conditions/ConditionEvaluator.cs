using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    public abstract class ConditionEvaluator<Agent, TInput> : ISerializableBT
        where TInput : struct
    {
        public string name { get; private set; }

        public ConditionEvaluator(string name)
        {
            this.name = name;
        }

        public abstract bool Evaluate(TInput input);
    }
}
