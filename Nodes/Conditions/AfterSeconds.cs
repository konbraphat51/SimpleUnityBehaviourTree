using System;
using System.Reflection;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    /// <summary>
    /// Condition evaluator that returns true after the most recent action node 
    /// has been called continuously for a specified duration.
    /// Resets when a different action node is called.
    /// 
    /// Requirements for TSensory:
    /// - Must contain a 'deltaTime' field or property of type float
    /// - Must contain a 'currentActionName' field or property of type string
    /// </summary>
    /// <typeparam name="Agent">The agent type</typeparam>
    /// <typeparam name="TSensory">The sensory input struct type</typeparam>
    [SerializableEvaluator("AfterSeconds")]
    public class AfterSeconds<Agent, TSensory> : ConditionEvaluator<Agent, TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("seconds")]
        public float seconds { get; private set; }

        private string _lastActionName = null;
        private float _accumulatedTime = 0f;

        // Cache reflection metadata for performance
        private static FieldInfo _deltaTimeField;
        private static PropertyInfo _deltaTimeProperty;
        private static FieldInfo _actionNameField;
        private static PropertyInfo _actionNameProperty;
        private static bool _reflectionInitialized = false;

        public AfterSeconds(float seconds)
            : base("AfterSeconds")
        {
            this.seconds = seconds;
            InitializeReflection();
        }

        public override bool Evaluate(TSensory input)
        {
            // Extract deltaTime from input using cached reflection metadata
            float deltaTime = GetDeltaTime(input);

            // Get the current action name from the input
            string currentActionName = GetCurrentActionName(input);

            if (string.IsNullOrEmpty(currentActionName))
            {
                // No action node is currently executing
                _accumulatedTime = 0f;
                _lastActionName = null;
                return false;
            }

            // Check if the action node has changed
            if (_lastActionName != currentActionName)
            {
                // Different action node - reset the timer
                _lastActionName = currentActionName;
                _accumulatedTime = 0f;
            }

            // Accumulate time
            _accumulatedTime += deltaTime;

            // Return true if the accumulated time exceeds the threshold
            return _accumulatedTime >= seconds;
        }

        private static void InitializeReflection()
        {
            if (_reflectionInitialized)
                return;

            // Cache deltaTime field/property
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

            // Cache currentActionName field/property
            _actionNameField = typeof(TSensory).GetField("currentActionName");
            if (_actionNameField == null || _actionNameField.FieldType != typeof(string))
            {
                _actionNameField = null;
                _actionNameProperty = typeof(TSensory).GetProperty("currentActionName");
                if (_actionNameProperty == null || _actionNameProperty.PropertyType != typeof(string))
                {
                    _actionNameProperty = null;
                }
            }

            _reflectionInitialized = true;
        }

        private float GetDeltaTime(TSensory input)
        {
            // Use cached reflection metadata
            if (_deltaTimeField != null)
            {
                return (float)_deltaTimeField.GetValue(input);
            }

            if (_deltaTimeProperty != null)
            {
                return (float)_deltaTimeProperty.GetValue(input);
            }

            // If no deltaTime is found, return 0 (no time progression)
            return 0f;
        }

        private string GetCurrentActionName(TSensory input)
        {
            // Use cached reflection metadata
            if (_actionNameField != null)
            {
                return _actionNameField.GetValue(input) as string;
            }

            if (_actionNameProperty != null)
            {
                return _actionNameProperty.GetValue(input) as string;
            }

            // If no currentActionName is found, return null
            return null;
        }
    }
}
