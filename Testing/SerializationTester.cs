using BehaviorTree.Nodes;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Sample
{
    public class SerializationTester : MonoBehaviour
    {
        // Define test input/output structs for serialization testing
        public struct TestSensory
        {
            public int value;
        }

        public struct TestAction
        {
            public Node<int, TestSensory, TestAction>.State state;
        }

        void Start()
        {
            Node<int, TestSensory, TestAction> root = new Sequence<int, TestSensory, TestAction>(
                new Node<int, TestSensory, TestAction>[]
                {
                    new Condition<int, TestSensory, TestAction>(
                        new And<int, TestSensory>(
                            new ConditionEvaluator<int, TestSensory>[]
                            {
                                new Not<int, TestSensory>(new SampleEvaluator<int, TestSensory>(10, 1.5f)),
                                new SampleEvaluator<int, TestSensory>(20, 2.5f),
                            }
                        ),
                        new SampleAction<int, TestSensory, TestAction>(1, 2.0f),
                        new SampleAction<int, TestSensory, TestAction>(2, 3.0f)
                    ),
                    new SampleAction<int, TestSensory, TestAction>(0, 0.0f),
                }
            );

            string serializedTree = Serializer<int, TestSensory, TestAction>.WriteNodeJson(root);
            Debug.Log(serializedTree);
            Node<int, TestSensory, TestAction> deserializedTree = Deserializer<int, TestSensory, TestAction>.ReadNodeJson(serializedTree);
            string reserializedTree = Serializer<int, TestSensory, TestAction>.WriteNodeJson(deserializedTree);
            Debug.Log(reserializedTree);
            Debug.Log(serializedTree == reserializedTree);
        }
    }
}
