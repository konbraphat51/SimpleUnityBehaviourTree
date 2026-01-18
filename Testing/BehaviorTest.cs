using System;
using BehaviorTree.Nodes;

namespace BehaviorTree.Testing
{
    /// <summary>
    /// Test to verify the behavior tree re-evaluates conditions every frame
    /// and maintains Sequence state properly.
    /// </summary>
    public class BehaviorTest
    {
        private class TestAgent
        {
            public bool conditionValue = false;
            public int actionExecutionCount = 0;
            public int action1Count = 0;
            public int action2Count = 0;
            public int sequenceAction1Count = 0;
            public int sequenceAction2Count = 0;
        }

        private class TestEvaluator : ConditionEvaluator<TestAgent>
        {
            public TestEvaluator() : base("TestEvaluator") { }

            public override bool Evaluate(TestAgent agent)
            {
                return agent.conditionValue;
            }
        }

        private class TestAction : Nodes.Action<TestAgent>
        {
            private int executionThreshold;
            private System.Action<TestAgent> onExecute;

            public TestAction(string name, int executionThreshold, System.Action<TestAgent> onExecute) 
                : base(name)
            {
                this.executionThreshold = executionThreshold;
                this.onExecute = onExecute;
            }

            protected override bool TakeAction(TestAgent agent)
            {
                onExecute?.Invoke(agent);
                agent.actionExecutionCount++;
                return agent.actionExecutionCount >= executionThreshold;
            }
        }

        public static void RunTests()
        {
            Console.WriteLine("Running Behavior Tree Tests...");
            
            TestConditionReEvaluation();
            TestSequenceStateMaintenance();
            TestSequenceResetOnDeselection();
            
            Console.WriteLine("All tests passed!");
        }

        /// <summary>
        /// Test that Condition nodes re-evaluate every frame and switch children dynamically
        /// </summary>
        private static void TestConditionReEvaluation()
        {
            Console.WriteLine("\nTest: Condition Re-evaluation");
            
            var agent = new TestAgent();
            var evaluator = new TestEvaluator();
            
            var actionTrue = new TestAction("ActionTrue", 3, a => a.action1Count++);
            var actionFalse = new TestAction("ActionFalse", 3, a => a.action2Count++);
            
            var condition = new Condition<TestAgent>(evaluator, actionTrue, actionFalse);
            
            // Start with condition false
            agent.conditionValue = false;
            
            // Tick 1: Should execute actionFalse
            var result = condition.Tick(agent);
            if (agent.action2Count != 1 || agent.action1Count != 0)
            {
                throw new Exception("Expected actionFalse to execute on tick 1");
            }
            
            // Tick 2: Change condition to true, should switch to actionTrue and reset actionFalse
            agent.conditionValue = true;
            agent.actionExecutionCount = 0; // Reset counter for new action
            result = condition.Tick(agent);
            if (agent.action1Count != 1 || agent.action2Count != 1)
            {
                throw new Exception("Expected actionTrue to execute after condition change");
            }
            
            // Tick 3: Condition still true, continue with actionTrue
            result = condition.Tick(agent);
            if (agent.action1Count != 2)
            {
                throw new Exception("Expected actionTrue to continue executing");
            }
            
            Console.WriteLine("✓ Condition re-evaluation test passed");
        }

        /// <summary>
        /// Test that Sequence nodes maintain state across frames
        /// </summary>
        private static void TestSequenceStateMaintenance()
        {
            Console.WriteLine("\nTest: Sequence State Maintenance");
            
            var agent = new TestAgent();
            
            var action1 = new TestAction("SeqAction1", 2, a => a.sequenceAction1Count++);
            var action2 = new TestAction("SeqAction2", 2, a => a.sequenceAction2Count++);
            
            var sequence = new Sequence<TestAgent>(new Node<TestAgent>[] { action1, action2 });
            
            // Tick 1: Execute first action
            var result = sequence.Tick(agent);
            if (result != Node<TestAgent>.State.RUNNING || agent.sequenceAction1Count != 1)
            {
                throw new Exception("Expected sequence to start with first action");
            }
            
            // Tick 2: Continue first action until it completes
            result = sequence.Tick(agent);
            if (result != Node<TestAgent>.State.RUNNING || agent.sequenceAction1Count != 2)
            {
                throw new Exception("Expected first action to complete");
            }
            
            // Tick 3: First action completed, move to second action
            agent.actionExecutionCount = 0; // Reset for second action
            result = sequence.Tick(agent);
            if (result != Node<TestAgent>.State.RUNNING || agent.sequenceAction2Count != 1)
            {
                throw new Exception("Expected sequence to move to second action");
            }
            
            // Tick 4: Complete second action
            result = sequence.Tick(agent);
            if (result != Node<TestAgent>.State.SUCCESS || agent.sequenceAction2Count != 2)
            {
                throw new Exception("Expected sequence to complete successfully");
            }
            
            Console.WriteLine("✓ Sequence state maintenance test passed");
        }

        /// <summary>
        /// Test that Sequence nodes reset when deselected (e.g., condition changes)
        /// </summary>
        private static void TestSequenceResetOnDeselection()
        {
            Console.WriteLine("\nTest: Sequence Reset on Deselection");
            
            var agent = new TestAgent();
            var evaluator = new TestEvaluator();
            
            var seqAction1 = new TestAction("SeqAction1", 3, a => a.sequenceAction1Count++);
            var seqAction2 = new TestAction("SeqAction2", 3, a => a.sequenceAction2Count++);
            var sequenceTrue = new Sequence<TestAgent>(new Node<TestAgent>[] { seqAction1, seqAction2 });
            
            var actionFalse = new TestAction("ActionFalse", 1, a => a.action2Count++);
            
            var condition = new Condition<TestAgent>(evaluator, sequenceTrue, actionFalse);
            
            // Start with condition true
            agent.conditionValue = true;
            
            // Tick 1: Start sequence, execute first action
            var result = condition.Tick(agent);
            if (agent.sequenceAction1Count != 1)
            {
                throw new Exception("Expected sequence to start");
            }
            if (sequenceTrue.childCurrent != 0)
            {
                throw new Exception("Expected sequence to be at first child");
            }
            
            // Tick 2: Continue first action
            result = condition.Tick(agent);
            if (agent.sequenceAction1Count != 2)
            {
                throw new Exception("Expected sequence to continue first action");
            }
            
            // Tick 3: Change condition to false, sequence should be reset
            agent.conditionValue = false;
            agent.actionExecutionCount = 0; // Reset for new action
            result = condition.Tick(agent);
            
            // Check that sequence was reset
            if (sequenceTrue.childCurrent != -1)
            {
                throw new Exception("Expected sequence to be reset when deselected");
            }
            
            // Verify actionFalse executed
            if (agent.action2Count != 1)
            {
                throw new Exception("Expected actionFalse to execute after condition change");
            }
            
            // Tick 4: Change condition back to true, sequence should start from beginning
            agent.conditionValue = true;
            agent.actionExecutionCount = 0; // Reset
            result = condition.Tick(agent);
            
            if (sequenceTrue.childCurrent != 0)
            {
                throw new Exception("Expected sequence to restart from beginning");
            }
            if (agent.sequenceAction1Count != 3)
            {
                throw new Exception("Expected sequence to restart first action");
            }
            
            Console.WriteLine("✓ Sequence reset on deselection test passed");
        }
    }
}
