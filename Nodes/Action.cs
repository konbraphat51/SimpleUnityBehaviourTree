namespace BehaviorTree.Nodes
{
    public abstract class Action<Agent, TSensory, TAction> : Node<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public Action(string name)
            : base(name) { }

        public override TAction Tick(TSensory input)
        {
            return TakeAction(input);
        }

        protected abstract TAction TakeAction(TSensory input);
    }
}
