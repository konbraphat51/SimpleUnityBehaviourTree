namespace BehaviorTree.Nodes
{
    /// <summary>
    /// Interface for condition evaluators that need access to the BehaviorTree.
    /// </summary>
    internal interface IBehaviorTreeAware<Agent, TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        void SetBehaviorTree(BehaviorTree<Agent, TSensory, TAction> tree);
    }
}
