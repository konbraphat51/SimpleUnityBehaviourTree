using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

namespace BehaviorTree.Sample
{
    [SerializableNode("SampleAction")]
    public class SampleAction<Agent, TInput, TOutput> : Action<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        [ConstructorParameter("p0")]
        public int p0 { get; private set; }

        [ConstructorParameter("p1")]
        public float p1 { get; private set; }

        public SampleAction(int p0, float p1)
            : base("SampleAction")
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        protected override TOutput TakeAction(TInput input)
        {
            // Implement your action logic here
            return default(TOutput); // Return appropriate output struct
        }
    }
}
