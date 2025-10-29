namespace BehaviorTree.Nodes
{
    public abstract class Action<Agent> : Node<Agent>
    {
        public Action(string name)
            : base(name) { }

        public override State Tick(Agent agent)
        {
            return TakeAction(agent) ? State.SUCCESS : State.RUNNING;
        }

        protected abstract bool TakeAction(Agent agent);
    }
}
