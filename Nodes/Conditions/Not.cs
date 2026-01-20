using System.Collections.Generic;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableEvaluator("Not")]
    public class Not<Agent, TInput> : Logic<Agent, TInput>
        where TInput : struct
    {
        [ConstructorParameter("condition")]
        public ConditionEvaluator<Agent, TInput> condition
        {
            get { return _children[0]; }
        }

        public Not(ConditionEvaluator<Agent, TInput> condition)
            : base("Not")
        {
            _children = new List<ConditionEvaluator<Agent, TInput>> { condition };
        }

        public override bool Evaluate(TInput input)
        {
            return !_children[0].Evaluate(input);
        }
    }
}
