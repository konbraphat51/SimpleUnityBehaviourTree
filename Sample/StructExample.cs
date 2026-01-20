using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

namespace BehaviorTree.Sample
{
    // Example of TInput struct - contains data passed to nodes during Tick
    public struct BehaviorInput
    {
        public float deltaTime;
        public bool isPlayerNear;
        public float health;
    }

    // Example of TOutput struct - contains result data from node execution
    public struct BehaviorOutput
    {
        public Node<object, BehaviorInput, BehaviorOutput>.State state;
        public string message;
        public float damageDealt;

        public static BehaviorOutput Success(string message = "")
        {
            return new BehaviorOutput
            {
                state = Node<object, BehaviorInput, BehaviorOutput>.State.SUCCESS,
                message = message,
                damageDealt = 0
            };
        }

        public static BehaviorOutput Running(string message = "")
        {
            return new BehaviorOutput
            {
                state = Node<object, BehaviorInput, BehaviorOutput>.State.RUNNING,
                message = message,
                damageDealt = 0
            };
        }

        public static BehaviorOutput Failure(string message = "")
        {
            return new BehaviorOutput
            {
                state = Node<object, BehaviorInput, BehaviorOutput>.State.FAILURE,
                message = message,
                damageDealt = 0
            };
        }
    }

    // Example custom action using the struct-based API
    [SerializableNode("AttackAction")]
    public class AttackAction : Action<object, BehaviorInput, BehaviorOutput>
    {
        [ConstructorParameter("damage")]
        public float damage { get; private set; }

        public AttackAction(float damage) : base("AttackAction")
        {
            this.damage = damage;
        }

        protected override BehaviorOutput TakeAction(BehaviorInput input)
        {
            if (input.isPlayerNear)
            {
                return new BehaviorOutput
                {
                    state = Node<object, BehaviorInput, BehaviorOutput>.State.SUCCESS,
                    message = $"Attacked player for {damage} damage",
                    damageDealt = damage
                };
            }
            else
            {
                return BehaviorOutput.Running("Player not in range");
            }
        }
    }

    // Example custom condition evaluator using the struct-based API
    [SerializableEvaluator("HealthCheck")]
    public class HealthCheck : ConditionEvaluator<object, BehaviorInput>
    {
        [ConstructorParameter("threshold")]
        public float threshold { get; private set; }

        public HealthCheck(float threshold) : base("HealthCheck")
        {
            this.threshold = threshold;
        }

        public override bool Evaluate(BehaviorInput input)
        {
            return input.health > threshold;
        }
    }

    // Example of extending Sequence to handle state extraction from TOutput
    [SerializableNode("StructSequence")]
    public class StructSequence : Sequence<object, BehaviorInput, BehaviorOutput>
    {
        public StructSequence(Node<object, BehaviorInput, BehaviorOutput>[] children)
            : base(children) { }

        protected override BehaviorOutput CreateSuccessOutput(BehaviorInput input)
        {
            return BehaviorOutput.Success("Sequence completed");
        }

        protected override BehaviorOutput CreateRunningOutput(BehaviorInput input)
        {
            return BehaviorOutput.Running("Sequence running");
        }

        protected override Node<object, BehaviorInput, BehaviorOutput>.State GetStateFromOutput(BehaviorOutput output)
        {
            return output.state;
        }
    }
}
