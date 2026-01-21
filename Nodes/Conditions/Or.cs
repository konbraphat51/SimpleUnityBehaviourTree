using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Or")]
    public class Or<Agent, TSensory> : Logic<Agent, TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<Agent, TSensory>[] conditioonsArray
        {
            get { return _children.ToArray(); }
        }

        public Or(ConditionEvaluator<Agent, TSensory>[] conditions)
            : base("Or")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TSensory input, BtInformation btInfo)
        {
            foreach (ConditionEvaluator<Agent, TSensory> condition in _children)
            {
                if (condition.Evaluate(input, btInfo))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
