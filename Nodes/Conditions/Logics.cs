using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BehaviorTree.Nodes
{
    public abstract class Logic<Agent> : ConditionEvaluator<Agent>
    {
        protected List<ConditionEvaluator<Agent>> _children = new List<ConditionEvaluator<Agent>>();
        public ReadOnlyCollection<ConditionEvaluator<Agent>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Logic(string name)
            : base(name) { }
    }
}
