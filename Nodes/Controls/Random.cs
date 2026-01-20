using System;
using System.Collections.Generic;
using System.IO;
using BehaviorTree.Serializations;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Random")]
    public class Random<Agent, TInput, TOutput> : Node<Agent, TInput, TOutput>
        where TInput : struct
        where TOutput : struct
    {
        public struct ChildWithWeight
        {
            public Node<Agent, TInput, TOutput> child;
            public float weight;
        }

        protected List<float> _weights = new List<float>();
        public IReadOnlyList<float> weights
        {
            get { return _weights.AsReadOnly(); }
        }
        public Node<Agent, TInput, TOutput> nodeSelected { get; private set; } = null;

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
        public Node<Agent, TInput, TOutput>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        [ConstructorParameter("weights")]
        public float[] weightsArray
        {
            get { return _weights.ToArray(); }
        }

        public Random(Node<Agent, TInput, TOutput>[] children, float[] weights)
            : base("Random")
        {
            _children.AddRange(children);
            _weights.AddRange(weights);

            if (_children.Count != _weights.Count)
            {
                throw new InvalidDataException("Number of children and weights must be the same.");
            }
        }

        public Random(Dictionary<Node<Agent, TInput, TOutput>, float> childrenWithWeights)
            : base("Random")
        {
            foreach (KeyValuePair<Node<Agent, TInput, TOutput>, float> pair in childrenWithWeights)
            {
                _children.Add(pair.Key);
                _weights.Add(pair.Value);
            }
        }

        public override TOutput Tick(TInput input)
        {
            // if not selected yet...
            TOutput result;
            if (nodeSelected == null)
            {
                result = SelectChildAndTick(input);
            }
            else
            {
                result = nodeSelected.Tick(input);
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

        public void AddChild(Node<Agent, TInput, TOutput> child, float weight)
        {
            _children.Add(child);
            _weights.Add(weight);
        }

        public void SetWeight(int index, float weight)
        {
            _weights[index] = weight;
        }

        public void SetWeight(Node<Agent, TInput, TOutput> child, float weight)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _weights[index] = weight;
            }
        }

        public void RemoveChild(Node<Agent, TInput, TOutput> child)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _children.RemoveAt(index);
                _weights.RemoveAt(index);
            }
        }

        protected TOutput SelectChildAndTick(TInput input)
        {
            Node<Agent, TInput, TOutput>[] shuffledChildren = ShuffleChildrenByWeights();
            foreach (Node<Agent, TInput, TOutput> child in shuffledChildren)
            {
                // try next child
                TOutput result = child.Tick(input);
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

        protected Node<Agent, TInput, TOutput>[] ShuffleChildrenByWeights()
        {
            Random random = new Random();
            List<Node<Agent, TInput, TOutput>> shuffled = new List<Node<Agent, TInput, TOutput>>();
            List<Node<Agent, TInput, TOutput>> childrenCopy = new List<Node<Agent, TInput, TOutput>>(_children);
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
        protected virtual TOutput CreateFailureOutput(TInput input)
        {
            // Default implementation - subclasses should override
            return default(TOutput);
        }

        protected virtual State GetStateFromOutput(TOutput output)
        {
            // Default implementation - subclasses should override
            // This assumes TOutput has a State field
            return State.SUCCESS;
        }
    }
}
