using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    /// <summary>
    /// Condition evaluator that returns true after the most recent action node 
    /// has been called continuously for a specified duration.
    /// Resets when a different action node is called.
    /// 
    /// Requires: The BehaviorTree must be configured with a deltaTime extractor.
    /// </summary>
    /// <typeparam name="Agent">The agent type</typeparam>
    /// <typeparam name="TSensory">The sensory input struct type</typeparam>
    /// <typeparam name="TAction">The action output struct type</typeparam>
    [SerializableEvaluator("AfterSeconds")]
    public class AfterSeconds<Agent, TSensory, TAction> : ConditionEvaluator<Agent, TSensory>, IBehaviorTreeAware<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        [ConstructorParameter("seconds")]
        public float seconds { get; private set; }

        private BehaviorTree<Agent, TSensory, TAction> _behaviorTree;

        public AfterSeconds(float seconds)
            : base("AfterSeconds")
        {
            this.seconds = seconds;
        }

        /// <summary>
        /// Sets the behavior tree reference for this evaluator.
        /// Called by the Condition node when setting up the tree.
        /// </summary>
        void IBehaviorTreeAware<Agent, TSensory, TAction>.SetBehaviorTree(BehaviorTree<Agent, TSensory, TAction> tree)
        {
            _behaviorTree = tree;
        }

        public override bool Evaluate(TSensory input)
        {
            if (_behaviorTree == null)
            {
                return false;
            }

            // Check if the current action has been running for the specified duration
            return _behaviorTree.currentActionElapsedTime >= seconds;
        }
    }
}

