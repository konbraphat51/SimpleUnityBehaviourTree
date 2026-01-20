using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BehaviorTree.Nodes
{
    public abstract class Logic<Agent, TSensory> : ConditionEvaluator<Agent, TSensory>
        where TSensory : struct
    {
        protected List<ConditionEvaluator<Agent, TSensory>> _children = new List<ConditionEvaluator<Agent, TSensory>>();
        public ReadOnlyCollection<ConditionEvaluator<Agent, TSensory>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Logic(string name)
            : base(name) { }
    }
}
