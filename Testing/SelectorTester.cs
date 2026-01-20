using BehaviorTree.Nodes;
using BehaviorTree.Serializations;
using UnityEngine;

namespace BehaviorTree.Sample
{
    public class SelectorTester : MonoBehaviour
    {
        // Define test input/output structs for Selector testing
        public struct TestSensory
        {
            public int value;
        }

        public struct TestAction
        {
            public Node<int, TestSensory, TestAction>.State state;
        }

        // Test action that fails
        [SerializableNode("FailingAction")]
        public class FailingAction : Action<int, TestSensory, TestAction>
        {
            public FailingAction() : base("FailingAction") { }

            protected override TestAction TakeAction(TestSensory input)
            {
                Debug.Log("FailingAction: Returning FAILURE");
                return new TestAction { state = State.FAILURE };
            }
        }

        // Test action that succeeds
        [SerializableNode("SucceedingAction")]
        public class SucceedingAction : Action<int, TestSensory, TestAction>
        {
            public SucceedingAction() : base("SucceedingAction") { }

            protected override TestAction TakeAction(TestSensory input)
            {
                Debug.Log("SucceedingAction: Returning SUCCESS");
                return new TestAction { state = State.SUCCESS };
            }
        }

        // Custom Selector that implements the required abstract methods
        public class CustomSelector : Selector<int, TestSensory, TestAction>
        {
            public CustomSelector(Node<int, TestSensory, TestAction>[] children)
                : base(children) { }

            protected override TestAction CreateSuccessOutput(TestSensory input)
            {
                return new TestAction { state = State.SUCCESS };
            }

            protected override TestAction CreateFailureOutput(TestSensory input)
            {
                return new TestAction { state = State.FAILURE };
            }

            protected override TestAction CreateRunningOutput(TestSensory input)
            {
                return new TestAction { state = State.RUNNING };
            }

            protected override State GetStateFromOutput(TestAction output)
            {
                return output.state;
            }
        }

        void Start()
        {
            Debug.Log("=== Testing Selector Node ===");

            // Test 1: All children fail - should return FAILURE
            Debug.Log("\n--- Test 1: All children fail ---");
            var selector1 = new CustomSelector(
                new Node<int, TestSensory, TestAction>[]
                {
                    new FailingAction(),
                    new FailingAction(),
                    new FailingAction()
                }
            );
            var result1 = selector1.Tick(new TestSensory { value = 0 });
            Debug.Log($"Result: {result1.state} (Expected: FAILURE)");

            // Test 2: First child succeeds - should return SUCCESS
            Debug.Log("\n--- Test 2: First child succeeds ---");
            var selector2 = new CustomSelector(
                new Node<int, TestSensory, TestAction>[]
                {
                    new SucceedingAction(),
                    new FailingAction(),
                    new FailingAction()
                }
            );
            var result2 = selector2.Tick(new TestSensory { value = 0 });
            Debug.Log($"Result: {result2.state} (Expected: SUCCESS)");

            // Test 3: Second child succeeds - should try first (fail) then second (succeed)
            Debug.Log("\n--- Test 3: Second child succeeds ---");
            var selector3 = new CustomSelector(
                new Node<int, TestSensory, TestAction>[]
                {
                    new FailingAction(),
                    new SucceedingAction(),
                    new FailingAction()
                }
            );
            var result3 = selector3.Tick(new TestSensory { value = 0 });
            Debug.Log($"Result: {result3.state} (Expected: SUCCESS)");

            // Test 4: Empty selector - should return FAILURE
            Debug.Log("\n--- Test 4: Empty selector ---");
            var selector4 = new CustomSelector(new Node<int, TestSensory, TestAction>[] { });
            var result4 = selector4.Tick(new TestSensory { value = 0 });
            Debug.Log($"Result: {result4.state} (Expected: FAILURE)");

            // Test 5: Serialization test
            Debug.Log("\n--- Test 5: Serialization test ---");
            Node<int, TestSensory, TestAction> selectorRoot = new CustomSelector(
                new Node<int, TestSensory, TestAction>[]
                {
                    new FailingAction(),
                    new SucceedingAction()
                }
            );

            string serializedTree = Serializer<int, TestSensory, TestAction>.WriteNodeJson(selectorRoot);
            Debug.Log("Serialized Selector:");
            Debug.Log(serializedTree);

            Debug.Log("\n=== All Selector Tests Completed ===");
        }
    }
}
