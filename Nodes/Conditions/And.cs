using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("And")]
    public class And<Agent, TSensory> : Logic<Agent, TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent, TSensory>[] conditionsArray
        {
            get { return _children.ToArray(); }
        }

        public And(ConditionEvaluator<Agent, TSensory>[] conditions)
            : base("And")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TSensory input)
        {
            foreach (ConditionEvaluator<Agent, TSensory> condition in _children)
            {
                if (!condition.Evaluate(input))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddCondition(ConditionEvaluator<Agent, TSensory> condition)
        {
            _children.Add(condition);
        }
    }
}
