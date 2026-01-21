using System.Linq;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Or")]
    public class Or<TSensory> : Logic<TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<TSensory>[] conditioonsArray
        {
            get { return _children.ToArray(); }
        }

        public Or(ConditionEvaluator<TSensory>[] conditions)
            : base("Or")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TSensory input, BtInformation btInfo)
        {
            foreach (ConditionEvaluator<TSensory> condition in _children)
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
