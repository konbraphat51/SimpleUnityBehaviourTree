using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

namespace BehaviorTree.Sample
{
    [SerializableNode("SampleAction")]
    public class SampleAction<Agent> : Action<Agent>
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

        protected override bool TakeAction(Agent agent)
        {
            // Implement your action logic here
            return true; // Return true if action is successful
        }
    }
}
