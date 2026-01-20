using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("And")]
    public class And<Agent, TInput> : Logic<Agent, TInput>
        where TInput : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent, TInput>[] conditionsArray
        {
            get { return _children.ToArray(); }
        }

        public And(ConditionEvaluator<Agent, TInput>[] conditions)
            : base("And")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TInput input)
        {
            foreach (ConditionEvaluator<Agent, TInput> condition in _children)
            {
                if (!condition.Evaluate(input))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddCondition(ConditionEvaluator<Agent, TInput> condition)
        {
            _children.Add(condition);
        }
    }
}
