using BehaviorTree.Nodes;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Sample
{
    public class SerializationTester : MonoBehaviour
    {
        // Define test input/output structs for serialization testing
        public struct TestInput
        {
            public int value;
        }

        public struct TestOutput
        {
            public Node<int, TestInput, TestOutput>.State state;
        }

        void Start()
        {
            Node<int, TestInput, TestOutput> root = new Sequence<int, TestInput, TestOutput>(
                new Node<int, TestInput, TestOutput>[]
                {
                    new Condition<int, TestInput, TestOutput>(
                        new And<int, TestInput>(
                            new ConditionEvaluator<int, TestInput>[]
                            {
                                new Not<int, TestInput>(new SampleEvaluator<int, TestInput>(10, 1.5f)),
                                new SampleEvaluator<int, TestInput>(20, 2.5f),
                            }
                        ),
                        new SampleAction<int, TestInput, TestOutput>(1, 2.0f),
                        new SampleAction<int, TestInput, TestOutput>(2, 3.0f)
                    ),
                    new SampleAction<int, TestInput, TestOutput>(0, 0.0f),
                }
            );

            string serializedTree = Serializer<int, TestInput, TestOutput>.WriteNodeJson(root);
            Debug.Log(serializedTree);
            Node<int, TestInput, TestOutput> deserializedTree = Deserializer<int, TestInput, TestOutput>.ReadNodeJson(serializedTree);
            string reserializedTree = Serializer<int, TestInput, TestOutput>.WriteNodeJson(deserializedTree);
            Debug.Log(reserializedTree);
            Debug.Log(serializedTree == reserializedTree);
        }
    }
}
