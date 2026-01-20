using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Or")]
    public class Or<Agent, TInput> : Logic<Agent, TInput>
        where TInput : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent, TInput>[] conditioonsArray
        {
            get { return _children.ToArray(); }
        }

        public Or(ConditionEvaluator<Agent, TInput>[] conditions)
            : base("Or")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TInput input)
        {
            foreach (ConditionEvaluator<Agent, TInput> condition in _children)
            {
                if (condition.Evaluate(input))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
