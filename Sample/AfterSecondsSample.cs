using BehaviorTree.Nodes;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Sample
{
    /// <summary>
    /// Sample demonstrating the AfterSeconds condition evaluator.
    /// This example shows how to use AfterSeconds to detect when an action
    /// has been running continuously for a specified duration.
    /// </summary>
    public class AfterSecondsSample : MonoBehaviour
    {
        // Define sensory input struct
        public struct TimedSensory
        {
            public float value;  // Example data
        }

        // Define action output struct
        public struct TimedAction
        {
            public Node<int, TimedSensory, TimedAction>.State state;
            public string message;
        }

        private BehaviorTree<int, TimedSensory, TimedAction> _tree;
        private float _testTimer = 0f;

        void Start()
        {
            // Create an AfterSeconds evaluator that returns true after 2 seconds
            var afterTwoSeconds = new AfterSeconds<int, TimedSensory>(2.0f);

            // Create sample actions
            var actionA = new TimedSampleAction<int, TimedSensory, TimedAction>("ActionA", 1.0f);
            var actionB = new TimedSampleAction<int, TimedSensory, TimedAction>("ActionB", 0.5f);

            // Create a condition that branches based on whether the action has run for 2 seconds
            var conditionNode = new Condition<int, TimedSensory, TimedAction>(
                afterTwoSeconds,
                actionB,  // If true (after 2 seconds), run ActionB
                actionA   // If false (before 2 seconds), run ActionA
            );

            // Create the behavior tree
            _tree = new BehaviorTree<int, TimedSensory, TimedAction>(
                "AfterSecondsDemo",
                conditionNode,
                0
            );

            Debug.Log("AfterSeconds sample started. ActionA will run, then after 2 seconds, ActionB will run.");
        }

        void Update()
        {
            _testTimer += Time.deltaTime;

            var input = new TimedSensory
            {
                value = _testTimer
            };

            // Tick the tree with deltaTime
            TimedAction output = _tree.Tick(input, Time.deltaTime);
            
            // Log every second for demonstration
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Time: {_testTimer:F2}s, State: {output.state}");
            }
        }
    }

    /// <summary>
    /// Sample action for demonstration
    /// </summary>
    [SerializableNode("TimedSampleAction")]
    public class TimedSampleAction<Agent, TSensory, TAction> : Action<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        [ConstructorParameter("actionName")]
        public string actionName { get; private set; }

        [ConstructorParameter("duration")]
        public float duration { get; private set; }

        public TimedSampleAction(string actionName, float duration)
            : base(actionName)
        {
            this.actionName = actionName;
            this.duration = duration;
        }

        protected override TAction TakeAction(TSensory input)
        {
            // Return running state
            // This is a simplified implementation
            return default(TAction);
        }
    }
}
