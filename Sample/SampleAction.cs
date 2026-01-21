using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

namespace BehaviorTree.Sample
{
    [SerializableNode("SampleAction")]
    public class SampleAction<TSensory, TAction> : Action<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
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

        protected override TAction TakeAction(TSensory input)
        {
            // Implement your action logic here
            return default(TAction); // Return appropriate output struct
        }
    }
}
