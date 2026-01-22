using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleUnityBehaviorTree.Nodes
{
    public abstract class Logic<TSensory> : ConditionEvaluator<TSensory>
        where TSensory : struct
    {
        protected List<ConditionEvaluator<TSensory>> _children =
            new List<ConditionEvaluator<TSensory>>();
        public ReadOnlyCollection<ConditionEvaluator<TSensory>> children
        {
            get { return _children.AsReadOnly(); }
        }

        public Logic(string name)
            : base(name) { }
    }
}
