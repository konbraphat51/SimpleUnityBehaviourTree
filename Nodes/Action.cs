namespace BehaviorTree.Nodes
{
    public abstract class Action<Agent, TInput, TOutput> : Node<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        public Action(string name)
            : base(name) { }

        public override TOutput Tick(TInput input)
        {
            return TakeAction(input);
        }

        protected abstract TOutput TakeAction(TInput input);
    }
}
