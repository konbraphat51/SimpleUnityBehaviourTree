using System.Linq;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    [SerializableEvaluator("And")]
    public class And<TSensory> : Logic<TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("conditions")]
        public ConditionEvaluator<TSensory>[] conditionsArray
        {
            get { return _children.ToArray(); }
        }

        public And(ConditionEvaluator<TSensory>[] conditions)
            : base("And")
        {
            _children = conditions.ToList();
        }

        public override bool Evaluate(TSensory input, BtInformation btInfo)
        {
            foreach (ConditionEvaluator<TSensory> condition in _children)
            {
                if (!condition.Evaluate(input, btInfo))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddCondition(ConditionEvaluator<TSensory> condition)
        {
            _children.Add(condition);
        }
    }
}
