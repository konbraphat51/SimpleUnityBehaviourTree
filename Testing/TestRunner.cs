using BehaviorTree.Testing;
using System;

namespace BehaviorTree
{
    /// <summary>
    /// Simple test runner that can be executed outside of Unity
    /// </summary>
    class TestRunner
    {
        static void Main(string[] args)
        {
            try
            {
                BehaviorTest.RunTests();
                Console.WriteLine("\n✓ All tests passed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Test failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
