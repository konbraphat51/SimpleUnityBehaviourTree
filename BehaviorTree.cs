using BehaviorTree.Nodes;
using System;

namespace BehaviorTree
{
    public class BehaviorTree<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public string name;
        public Node<Agent, TSensory, TAction> nodeRoot;

        private Agent _agent;
        public Agent agent
        {
            get { return _agent; }
            set
            {
                _agent = value;
                nodeRoot.Reset();
            }
        }

        // Track the current action node for time-based conditions
        private Action<Agent, TSensory, TAction> _currentAction = null;
        private float _currentActionElapsedTime = 0f;

        // Delegate to extract deltaTime from TSensory
        private Func<TSensory, float> _getDeltaTime;

        public Action<Agent, TSensory, TAction> currentAction
        {
            get { return _currentAction; }
        }

        public float currentActionElapsedTime
        {
            get { return _currentActionElapsedTime; }
        }

        public BehaviorTree(
            string name, 
            Node<Agent, TSensory, TAction> root, 
            Agent agent,
            Func<TSensory, float> getDeltaTime = null)
        {
            this.name = name;
            nodeRoot = root;
            this.agent = agent;
            _getDeltaTime = getDeltaTime;

            // Set behavior tree reference for all nodes
            nodeRoot.SetBehaviorTree(this);
        }

        public TAction Tick(TSensory input)
        {
            TAction result = nodeRoot.Tick(input);
            return result;
        }

        /// <summary>
        /// Called by Action nodes to register themselves as the current action.
        /// Updates the elapsed time tracking for AfterSeconds condition.
        /// </summary>
        internal void RegisterActionTick(Action<Agent, TSensory, TAction> action, TSensory input)
        {
            float deltaTime = 0f;
            if (_getDeltaTime != null)
            {
                deltaTime = _getDeltaTime(input);
            }

            if (_currentAction != action)
            {
                // Different action - reset timer
                _currentAction = action;
                _currentActionElapsedTime = 0f;
            }

            _currentActionElapsedTime += deltaTime;
        }
    }
}
