namespace BehaviorTree
{
    /// <summary>
    /// Information passed through the behavior tree during Tick.
    /// Contains time tracking data for time-based conditions like AfterSeconds.
    /// </summary>
    public struct BtInformation
    {
        /// <summary>
        /// Time elapsed since last tick (delta time).
        /// </summary>
        public float deltaTime;

        /// <summary>
        /// Name of the current action node being executed.
        /// Used by AfterSeconds to detect when actions change.
        /// </summary>
        public string currentActionName;

        /// <summary>
        /// Total time the current action has been executing continuously.
        /// </summary>
        public float currentActionElapsedTime;

        public BtInformation(float deltaTime)
        {
            this.deltaTime = deltaTime;
            this.currentActionName = null;
            this.currentActionElapsedTime = 0f;
        }
    }
}
