using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    /// <summary>
    /// Condition evaluator that returns true after the most recent action node 
    /// has been called continuously for a specified duration.
    /// Resets when a different action node is called.
    /// </summary>
    /// <typeparam name="TSensory">The sensory input struct type</typeparam>
    [SerializableEvaluator("AfterSeconds")]
    public class AfterSeconds<TSensory> : ConditionEvaluator<TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("seconds")]
        public float seconds { get; private set; }

        public AfterSeconds(float seconds)
            : base("AfterSeconds")
        {
            this.seconds = seconds;
        }

        public override bool Evaluate(TSensory input, BtInformation btInfo)
        {
            // Check if the current action has been running for the specified duration
            return btInfo.currentActionElapsedTime >= seconds;
        }
    }
}

