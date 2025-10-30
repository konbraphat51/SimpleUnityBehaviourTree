using System.Collections.Generic;
using System.IO;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Nodes
{
    [SerializableNode("Random")]
    public class Random<Agent> : Node<Agent>
    {
        public struct ChildWithWeight
        {
            public Node<Agent> child;
            public float weight;
        }

        protected List<float> _weights = new List<float>();
        public IReadOnlyList<float> weights
        {
            get { return _weights.AsReadOnly(); }
        }
        public Node<Agent> nodeSelected { get; private set; } = null;

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
        public Node<Agent>[] childrenArray
        {
            get { return _children.ToArray(); }
        }

        [ConstructorParameter("weights")]
        public float[] weightsArray
        {
            get { return _weights.ToArray(); }
        }

        public Random(Node<Agent>[] children, float[] weights)
            : base("Random")
        {
            _children.AddRange(children);
            _weights.AddRange(weights);

            if (_children.Count != _weights.Count)
            {
                throw new InvalidDataException("Number of children and weights must be the same.");
            }
        }

        public Random(Dictionary<Node<Agent>, float> childrenWithWeights)
            : base("Random")
        {
            foreach (KeyValuePair<Node<Agent>, float> pair in childrenWithWeights)
            {
                _children.Add(pair.Key);
                _weights.Add(pair.Value);
            }
        }

        public override State Tick(Agent agent)
        {
            // if not selected yet...
            State result;
            if (nodeSelected == null)
            {
                result = SelectChildAndTick(agent);
            }
            else
            {
                result = nodeSelected.Tick(agent);
            }

            // if finished...
            if (result != State.RUNNING)
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

        public void AddChild(Node<Agent> child, float weight)
        {
            _children.Add(child);
            _weights.Add(weight);
        }

        public void SetWeight(int index, float weight)
        {
            _weights[index] = weight;
        }

        public void SetWeight(Node<Agent> child, float weight)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _weights[index] = weight;
            }
        }

        public void RemoveChild(Node<Agent> child)
        {
            int index = _children.IndexOf(child);
            if (index >= 0)
            {
                _children.RemoveAt(index);
                _weights.RemoveAt(index);
            }
        }

        protected State SelectChildAndTick(Agent agent)
        {
            Node<Agent>[] shuffledChildren = ShuffleChildrenByWeights();
            foreach (Node<Agent> child in shuffledChildren)
            {
                // try next child
                State result = child.Tick(agent);

                // if not failed...
                if (result != State.FAILURE)
                {
                    // ... select this child
                    nodeSelected = child;
                    return result;
                }

                // select next node if failed
            }

            // all children failed
            return State.FAILURE;
        }

        protected Node<Agent>[] ShuffleChildrenByWeights()
        {
            List<Node<Agent>> shuffled = new List<Node<Agent>>();
            List<Node<Agent>> childrenCopy = new List<Node<Agent>>(_children);
            List<float> weightsCopy = new List<float>(_weights);
            while (childrenCopy.Count > 0)
            {
                float totalWeight = 0f;
                foreach (float weight in weightsCopy)
                {
                    totalWeight += weight;
                }

                float randomValue = Random.Range(0f, totalWeight);
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
    }
}
