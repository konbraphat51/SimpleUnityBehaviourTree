using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Not")]
    public class Not<Agent, TSensory> : Logic<Agent, TSensory>
        where TSensory : struct
    {
        [ConstructorParameter("condition")]
        public ConditionEvaluator<Agent, TSensory> condition
        {
            get { return _children[0]; }
        }

        public Not(ConditionEvaluator<Agent, TSensory> condition)
            : base("Not")
        {
            _children = new List<ConditionEvaluator<Agent, TSensory>> { condition };
        }

        public override bool Evaluate(TSensory input)
        {
            return !_children[0].Evaluate(input);
        }
    }
}
