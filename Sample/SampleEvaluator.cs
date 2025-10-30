using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

namespace BehaviorTree.Sample
{
    [SerializableEvaluator("SampleEvaluator")]
    public class SampleEvaluator<Agent> : ConditionEvaluator<Agent>
    {
        [ConstructorParameter("p0")]
        public int p0 { get; private set; }

        [ConstructorParameter("p1")]
        public float p1 { get; private set; }

        public SampleEvaluator(int p0, float p1)
            : base("SampleEvaluator")
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public override bool Evaluate(Agent agent)
        {
            // Sample evaluation logic
            return true;
        }
    }
}
