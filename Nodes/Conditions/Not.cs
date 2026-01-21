using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Not")]
    public class Not<TSensory> : Logic<TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("condition")]
        public ConditionEvaluator<TSensory> condition
        {
            get { return _children[0]; }
        }

        public Not(ConditionEvaluator<TSensory> condition)
            : base("Not")
        {
            _children = new List<ConditionEvaluator<TSensory>> { condition };
        }

        public override bool Evaluate(TSensory input, BtInformation btInfo)
        {
            return !_children[0].Evaluate(input, btInfo);
        }
    }
}
