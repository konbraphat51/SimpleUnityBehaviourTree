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

        public AfterSeconds(float seconds)
            : base("AfterSeconds")
        {
            this.seconds = seconds;
        }

        public override bool Evaluate(TSensory input)
        {
            // Extract deltaTime from input using reflection
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

        /// <summary>
        /// Resets the accumulated time and last action name.
        /// </summary>
        public void Reset()
        {
            _accumulatedTime = 0f;
            _lastActionName = null;
        }

        private float GetDeltaTime(TSensory input)
        {
            // Use reflection to get deltaTime field from the input struct
            var deltaTimeField = typeof(TSensory).GetField("deltaTime");
            if (deltaTimeField != null && deltaTimeField.FieldType == typeof(float))
            {
                return (float)deltaTimeField.GetValue(input);
            }

            // Fallback to a property
            var deltaTimeProperty = typeof(TSensory).GetProperty("deltaTime");
            if (deltaTimeProperty != null && deltaTimeProperty.PropertyType == typeof(float))
            {
                return (float)deltaTimeProperty.GetValue(input);
            }

            // If no deltaTime is found, return 0 (no time progression)
            return 0f;
        }

        private string GetCurrentActionName(TSensory input)
        {
            // Use reflection to get currentActionName field from the input struct
            var actionNameField = typeof(TSensory).GetField("currentActionName");
            if (actionNameField != null && actionNameField.FieldType == typeof(string))
            {
                return actionNameField.GetValue(input) as string;
            }

            // Fallback to a property
            var actionNameProperty = typeof(TSensory).GetProperty("currentActionName");
            if (actionNameProperty != null && actionNameProperty.PropertyType == typeof(string))
            {
                return actionNameProperty.GetValue(input) as string;
            }

            // If no currentActionName is found, return null
            return null;
        }
    }
}
