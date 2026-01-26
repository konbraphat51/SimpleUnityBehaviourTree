namespace SimpleUnityBehaviorTree.Nodes
{
    public abstract class Action<TSensory, TAction> : Node<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public Action(string name)
            : base(name) { }

        public override (bool, TAction) Tick(TSensory input, BtInformation btInfo)
        {
            // Update action tracking in BtInformation
            if (btInfo.currentActionName != name)
            {
                // Different action - reset timer
                btInfo.currentActionName = name;
                btInfo.currentActionElapsedTime = 0f;
            }

            btInfo.currentActionElapsedTime += btInfo.deltaTime;

            TAction action = TakeAction(input);
            // Action nodes always return true (action executed)
            return (true, action);
        }

        protected abstract TAction TakeAction(TSensory input);
    }
}
