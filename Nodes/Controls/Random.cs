using System;
using System.Collections.Generic;
using System.IO;
using SimpleUnityBehaviorTree.Serializations;

namespace SimpleUnityBehaviorTree.Nodes
{
    [SerializableNode("Random")]
    public class Random<TSensory, TAction> : Node<TSensory, TAction>
        where TSensory : struct
        where TAction : struct
    {
        public struct ChildWithWeight
        {
            public Node<TSensory, TAction> child;
            public float weight;
        }

        protected List<float> _weights = new List<float>();
        public IReadOnlyList<float> weights
        {
            get { return _weights.AsReadOnly(); }
        }
        public Node<TSensory, TAction> nodeSelected { get; private set; } = null;

        public ChildWithWeight[] childrenWithWeights
        {
            get
            {
                List<ChildWithWeight> result = new List<ChildWithWeight>();
                for (int cnt = 0; cnt < _children.Count; cnt++)
                {
                    result.Add(
                        new ChildWithWeight { child = _children[cnt], weight = _weights[cnt] }
                    );
                }
                return result.ToArray();
            }
        }

        [ConstructorParameter("children")]
        public Node<TSensory, TAction>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        [ConstructorParameter("weights")]
        public float[] weightsArray
        {
            get { return _weights.ToArray(); }
        }

        public Random(Node<TSensory, TAction>[] children, float[] weights)
            : base("Random")
        {
            _children.AddRange(children);
            _weights.AddRange(weights);

            if (_children.Count != _weights.Count)
            {
                throw new InvalidDataException("Number of children and weights must be the same.");
            }
        }

        public Random(Dictionary<Node<TSensory, TAction>, float> childrenWithWeights)
            : base("Random")
        {
            foreach (KeyValuePair<Node<TSensory, TAction>, float> pair in childrenWithWeights)
            {
                _children.Add(pair.Key);
                _weights.Add(pair.Value);
            }
        }

        public override TAction Tick(TSensory input, BtInformation btInfo)
        {
            // if not selected yet...
            TAction result;
            if (nodeSelected == null)
            {
                result = SelectChildAndTick(input, btInfo);
            }
            else
            {
                result = nodeSelected.Tick(input, btInfo);
            }

            // Check the state of the result
            State resultState = GetStateFromOutput(result);

            // if finished...
            if (resultState != State.RUNNING)
            {
                // reset when done
                Reset();
            }

            return result;
        }

        public override void Reset()
        {
            base.Reset();
            nodeSelected = null;
        }

        public void AddChild(Node<TSensory, TAction> child, float weight)
        {
            _children.Add(child);
            _weights.Add(weight);
        }

        public void SetWeight(int index, float weight)
        {
            _weights[index] = weight;
        }

        public void SetWeight(Node<TSensory, TAction> child, float weight)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _weights[index] = weight;
            }
        }

        public void RemoveChild(Node<TSensory, TAction> child)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _children.RemoveAt(index);
                _weights.RemoveAt(index);
            }
        }

        protected TAction SelectChildAndTick(TSensory input, BtInformation btInfo)
        {
            Node<TSensory, TAction>[] shuffledChildren = ShuffleChildrenByWeights();
            foreach (Node<TSensory, TAction> child in shuffledChildren)
            {
                // try next child
                TAction result = child.Tick(input, btInfo);
                State resultState = GetStateFromOutput(result);

                // if not failed...
                if (resultState != State.FAILURE)
                {
                    // ... select this child
                    nodeSelected = child;
                    return result;
                }

                // select next node if failed
            }

            // all children failed
            return CreateFailureOutput(input);
        }

        protected Node<TSensory, TAction>[] ShuffleChildrenByWeights()
        {
            Random random = new Random();
            List<Node<TSensory, TAction>> shuffled = new List<Node<TSensory, TAction>>();
            List<Node<TSensory, TAction>> childrenCopy = new List<Node<TSensory, TAction>>(
                _children
            );
            List<float> weightsCopy = new List<float>(_weights);
            while (childrenCopy.Count > 0)
            {
                float totalWeight = 0f;
                foreach (float weight in weightsCopy)
                {
                    totalWeight += weight;
                }

                float randomValue = ComputeRandom(totalWeight, random);
                float cumulativeWeight = 0f;
                for (int cnt = 0; cnt < childrenCopy.Count; cnt++)
                {
                    cumulativeWeight += weightsCopy[cnt];
                    if (randomValue <= cumulativeWeight)
                    {
                        shuffled.Add(childrenCopy[cnt]);
                        childrenCopy.RemoveAt(cnt);
                        weightsCopy.RemoveAt(cnt);
                        break;
                    }
                }
            }
            return shuffled.ToArray();
        }

        private float ComputeRandom(float totalWeight, Random rand)
        {
            return (float)(rand.NextDouble() * totalWeight);
        }

        // Helper methods to create output with state
        protected virtual TAction CreateFailureOutput(TSensory input)
        {
            // Default implementation - subclasses should override
            return default(TAction);
        }

        protected virtual State GetStateFromOutput(TAction output)
        {
            // Default implementation - subclasses should override
            // This assumes TAction has a State field
            return State.SUCCESS;
        }
    }
}
