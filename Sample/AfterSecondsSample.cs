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
        // Define sensory input struct with required fields for AfterSeconds
        public struct TimedSensory
        {
            public float deltaTime;           // Required for time tracking
            public string currentActionName;  // Required to identify the current action
            public float value;               // Example additional data
        }

        // Define action output struct
        public struct TimedAction
        {
            public Node<int, TimedSensory, TimedAction>.State state;
            public string message;
        }

        private BehaviorTree<int, TimedSensory, TimedAction> _tree;
        private string _currentAction = null;
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

            // Simulate action changes at specific times for testing
            if (_testTimer < 5f)
            {
                _currentAction = "ActionA";
            }
            else if (_testTimer < 7f)
            {
                _currentAction = "ActionB";
            }
            else
            {
                _currentAction = "ActionA";
                if (_testTimer > 12f)
                {
                    _testTimer = 0f; // Reset for demo loop
                }
            }

            var input = new TimedSensory
            {
                deltaTime = Time.deltaTime,
                currentActionName = _currentAction,
                value = _testTimer
            };

            TimedAction output = _tree.Tick(input);
            
            // Log every second for demonstration
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Time: {_testTimer:F2}s, Current Action: {_currentAction}, State: {output.state}");
            }
        }
    }

    /// <summary>
    /// Sample action that tracks its own name for use with AfterSeconds
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

        private float _elapsedTime = 0f;
        
        // Cache reflection metadata for performance
        private static System.Reflection.FieldInfo _deltaTimeField;
        private static System.Reflection.PropertyInfo _deltaTimeProperty;
        private static bool _reflectionInitialized = false;

        public TimedSampleAction(string actionName, float duration)
            : base(actionName)
        {
            this.actionName = actionName;
            this.duration = duration;
            InitializeReflection();
        }

        protected override TAction TakeAction(TSensory input)
        {
            // Extract deltaTime using cached reflection metadata
            float deltaTime = GetDeltaTime(input);
            _elapsedTime += deltaTime;

            // Return running or success based on duration
            // This is a simplified implementation
            return default(TAction);
        }

        public override void Reset()
        {
            base.Reset();
            _elapsedTime = 0f;
        }
        
        private static void InitializeReflection()
        {
            if (_reflectionInitialized)
                return;

            _deltaTimeField = typeof(TSensory).GetField("deltaTime");
            if (_deltaTimeField == null || _deltaTimeField.FieldType != typeof(float))
            {
                _deltaTimeField = null;
                _deltaTimeProperty = typeof(TSensory).GetProperty("deltaTime");
                if (_deltaTimeProperty == null || _deltaTimeProperty.PropertyType != typeof(float))
                {
                    _deltaTimeProperty = null;
                }
            }

            _reflectionInitialized = true;
        }

        private float GetDeltaTime(TSensory input)
        {
            if (_deltaTimeField != null)
            {
                return (float)_deltaTimeField.GetValue(input);
            }

            if (_deltaTimeProperty != null)
            {
                return (float)_deltaTimeProperty.GetValue(input);
            }

            return 0f;
        }
    }
}
