using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BehaviorTree.Nodes
{
    public abstract class Logic<Agent, TInput> : ConditionEvaluator<Agent, TInput>
        where TInput : struct
    {
        protected List<ConditionEvaluator<Agent, TInput>> _children = new List<ConditionEvaluator<Agent, TInput>>();
        public ReadOnlyCollection<ConditionEvaluator<Agent, TInput>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Logic(string name)
            : base(name) { }
    }
}
