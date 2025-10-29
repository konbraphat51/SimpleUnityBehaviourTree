namespace BehaviorTree.Nodes
{
    public abstract class Action<Agent> : Node<Agent>
    {
        public Action(string name)
            : base(name) { }
    }
}
