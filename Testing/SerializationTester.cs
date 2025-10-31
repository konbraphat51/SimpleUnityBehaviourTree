using BehaviorTree.Nodes;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Sample
{
    public class SerializationTester : MonoBehaviour
    {
        void Start()
        {
            Node<int> root = new Sequence<int>(
                new Node<int>[]
                {
                    new Condition<int>(
                        new And<int>(
                            new ConditionEvaluator<int>[]
                            {
                                new Not<int>(new SampleEvaluator<int>(10, 1.5f)),
                                new SampleEvaluator<int>(20, 2.5f),
                            }
                        ),
                        new SampleAction<int>(1, 2.0f),
                        new SampleAction<int>(2, 3.0f)
                    ),
                    new SampleAction<int>(0, 0.0f),
                }
            );

            string serializedTree = Serializer<int>.WriteNodeJson(root);
            Debug.Log(serializedTree);
            Node<int> deserializedTree = Deserializer<int>.ReadNodeJson(serializedTree);
            string reserializedTree = Serializer<int>.WriteNodeJson(deserializedTree);
            Debug.Log(reserializedTree);
            Debug.Log(serializedTree == reserializedTree);
        }
    }
}
