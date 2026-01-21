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
            public Node<TestSensory, TestAction>.State state;
        }

        void Start()
        {
            Node<TestSensory, TestAction> root = new Sequence<TestSensory, TestAction>(
                new Node<TestSensory, TestAction>[]
                {
                    new Condition<TestSensory, TestAction>(
                        new And<TestSensory>(
                            new ConditionEvaluator<TestSensory>[]
                            {
                                new Not<TestSensory>(new SampleEvaluator<TestSensory>(10, 1.5f)),
                                new SampleEvaluator<TestSensory>(20, 2.5f),
                            }
                        ),
                        new SampleAction<TestSensory, TestAction>(1, 2.0f),
                        new SampleAction<TestSensory, TestAction>(2, 3.0f)
                    ),
                    new SampleAction<TestSensory, TestAction>(0, 0.0f),
                }
            );

            string serializedTree = Serializer<TestSensory, TestAction>.WriteNodeJson(root);
            Debug.Log(serializedTree);
            Node<TestSensory, TestAction> deserializedTree = Deserializer<TestSensory, TestAction>.ReadNodeJson(serializedTree);
            string reserializedTree = Serializer<TestSensory, TestAction>.WriteNodeJson(deserializedTree);
            Debug.Log(reserializedTree);
            Debug.Log(serializedTree == reserializedTree);
        }
    }
}
