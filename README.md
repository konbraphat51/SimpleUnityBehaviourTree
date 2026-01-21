# SimpleUnityBehaviourTree

A lightweight and flexible Behavior Tree implementation for Unity, designed with simplicity and extensibility in mind.

## Features

- **Struct-Based Input/Output**: Type-safe input and output structs for behavior tree execution
- **Core Node Types**: Includes essential behavior tree nodes (Sequence, Selector, Random, Condition, Action)
- **Condition Evaluators**: Built-in logic operators (And, Or, Not) for complex condition building
- **Serialization Support**: JSON serialization and deserialization for saving and loading behavior trees
- **Extensible Architecture**: Easy to create custom nodes and condition evaluators

## Installation

1. Copy the repository contents into your Unity project's `Assets` folder
2. Install Newtonsoft.Json (required for serialization):
   - Via Unity Package Manager: Add `com.unity.nuget.newtonsoft-json` from the Unity Registry
   - Or via NuGet: Install `Newtonsoft.Json` package

## Basic Usage

### Using Struct-Based Input/Output (Recommended)

The library now supports type-safe input and output structs for behavior tree execution:

```csharp
using BehaviorTree;
using BehaviorTree.Nodes;

// Define your input struct - data passed to nodes
public struct GameInput
{
    public bool isPlayerNear;
    public float health;
}

// Define your output struct - result from node execution
public struct GameOutput
{
    public Node<GameInput, GameOutput>.State state;
    public string message;
    
    public static GameOutput Success() => new GameOutput 
    { 
        state = Node<GameInput, GameOutput>.State.SUCCESS 
    };
    
    public static GameOutput Running() => new GameOutput 
    { 
        state = Node<GameInput, GameOutput>.State.RUNNING 
    };
    
    public static GameOutput Failure() => new GameOutput 
    { 
        state = Node<GameInput, GameOutput>.State.FAILURE 
    };
}

// Create custom action
public class AttackAction : Action<GameInput, GameOutput>
{
    private float damage;
    
    public AttackAction(float damage) : base("Attack")
    {
        this.damage = damage;
    }
    
    protected override GameOutput TakeAction(GameInput input)
    {
        if (input.isPlayerNear)
        {
            return GameOutput.Success();
        }
        return GameOutput.Running();
    }
}

// Build and use the tree
var root = new AttackAction(10f);
var tree = new BehaviorTree<GameInput, GameOutput>("AI", root);

void Update()
{
    var input = new GameInput 
    { 
        isPlayerNear = true,
        health = 100f
    };
    
    // Tick measures time internally using system clock
    GameOutput output = tree.Tick(input);
    Debug.Log($"State: {output.state}, Message: {output.message}");
}
```

### Creating Custom Actions

Extend the `Action<TSensory, TAction>` class to create custom actions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableNode("MoveToTarget")]
public class MoveToTarget : Action<GameInput, GameOutput>
{
    [ConstructorParameter("speed")]
    public float speed { get; private set; }

    public MoveToTarget(float speed) : base("MoveToTarget")
    {
        this.speed = speed;
    }

    protected override GameOutput TakeAction(GameInput input)
    {
        // Use input data to make decisions
        if (input.isPlayerNear)
        {
            // Return success output
            return GameOutput.Success();
        }
        // Return running output to continue execution
        return GameOutput.Running();
    }
}
```

### Creating Custom Condition Evaluators

Extend `ConditionEvaluator<TSensory>` for custom conditions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableEvaluator("IsHealthLow")]
public class IsHealthLow : ConditionEvaluator<GameInput>
{
    [ConstructorParameter("threshold")]
    public float threshold { get; private set; }

    public IsHealthLow(float threshold) : base("IsHealthLow")
    {
        this.threshold = threshold;
    }

    public override bool Evaluate(GameInput input)
    {
        return input.health < threshold;
    }
}
```

### Using Conditions with Logic Operators

```csharp
// Create condition evaluators
var isHealthLow = new IsHealthLow(30f);
var isEnemyNear = new IsEnemyNear(10f);

// Combine with logic operators
var shouldFlee = new And<GameInput>(new ConditionEvaluator<GameInput>[]
{
    isHealthLow,
    isEnemyNear
});

var shouldNotAttack = new Not<GameInput>(isEnemyNear);

var shouldHeal = new Or<GameInput>(new ConditionEvaluator<GameInput>[]
{
    isHealthLow,
    new IsPoisoned()
});

// Use in a Condition node - note that Condition nodes need custom implementation
// to handle state extraction from TAction
```

### Using AfterSeconds for Time-Based Conditions

The `AfterSeconds` evaluator returns true after an action has been running continuously for a specified duration. This is useful for implementing timeout behaviors, animations that need to complete, or state transitions based on duration.

**How it works:**
- BehaviorTree automatically tracks time using the system clock
- Action nodes automatically track when they're executing
- The timer resets when a different action is detected

```csharp
// Define sensory input
public struct GameInput
{
    public float health;
    public bool isPlayerNear;
}

// Create an AfterSeconds evaluator
var afterThreeSeconds = new AfterSeconds<GameInput>(3.0f);

// Use in a Condition node to switch behavior after 3 seconds
var timedCondition = new Condition<GameInput, GameOutput>(
    afterThreeSeconds,
    new SwitchTacticAction(),  // Run this after 3 seconds
    new DefaultAction()         // Run this before 3 seconds
);

// Create the behavior tree
var tree = new BehaviorTree<GameInput, GameOutput>(
    "MyTree",
    timedCondition
);

// In your update loop
void Update()
{
    var input = new GameInput
    {
        health = playerHealth,
        isPlayerNear = CheckPlayerProximity()
    };
    
    // Time is tracked automatically
    GameOutput output = tree.Tick(input);
}
```

**Note:** The timer resets automatically when a different action node starts executing, allowing you to create behaviors that depend on continuous action execution.

### Extending Control Nodes

When using struct-based I/O, control nodes like Sequence, Selector, and Random need to know how to extract the state from TAction:

```csharp
public class CustomSequence : Sequence<GameInput, GameOutput>
{
    public CustomSequence(Node<GameInput, GameOutput>[] children)
        : base(children) { }

    protected override GameOutput CreateSuccessOutput(GameInput input)
    {
        return GameOutput.Success();
    }

    protected override GameOutput CreateRunningOutput(GameInput input)
    {
        return GameOutput.Running();
    }

    protected override Node<GameInput, GameOutput>.State GetStateFromOutput(GameOutput output)
    {
        return output.state;
    }
}
```

### Selector Node

The `Selector` node executes children in order until one succeeds (also known as a Fallback node). You'll need to extend it to handle struct-based I/O:

```csharp
public class CustomSelector : Selector<GameInput, GameOutput>
{
    public CustomSelector(Node<GameInput, GameOutput>[] children)
        : base(children) { }

    protected override GameOutput CreateSuccessOutput(GameInput input)
    {
        return GameOutput.Success();
    }

    protected override GameOutput CreateFailureOutput(GameInput input)
    {
        return GameOutput.Failure();
    }

    protected override GameOutput CreateRunningOutput(GameInput input)
    {
        return GameOutput.Running();
    }

    protected override Node<GameInput, GameOutput>.State GetStateFromOutput(GameOutput output)
    {
        return output.state;
    }
}

// Usage
var selectorNode = new CustomSelector(
    new Node<GameInput, GameOutput>[] 
    {
        new TryAttackAction(),
        new TryFleeAction(),
        new IdleAction()
    }
);
```

### Random Selection Node

The `Random` node selects children based on weighted probability. You'll need to extend it to handle struct-based I/O:

```csharp
public class CustomRandom : Random<GameInput, GameOutput>
{
    public CustomRandom(Node<GameInput, GameOutput>[] children, float[] weights)
        : base(children, weights) { }

    protected override GameOutput CreateFailureOutput(GameInput input)
    {
        return GameOutput.Failure();
    }

    protected override Node<GameInput, GameOutput>.State GetStateFromOutput(GameOutput output)
    {
        return output.state;
    }
}

// Usage
var randomNode = new CustomRandom(
    new Node<GameInput, GameOutput>[] 
    {
        new PatrolAction(),
        new IdleAction(),
        new WanderAction()
    },
    new float[] { 0.5f, 0.3f, 0.2f } // Weights
);
```

## Node Types

### Core Nodes

| Node | Description |
|------|-------------|
| `Node<TSensory, TAction>` | Base abstract class for all nodes with struct-based I/O |
| `Action<TSensory, TAction>` | Base class for action/leaf nodes |
| `Sequence<TSensory, TAction>` | Executes children in order until one fails |
| `Selector<TSensory, TAction>` | Executes children in order until one succeeds |
| `Random<TSensory, TAction>` | Selects children randomly based on weights |
| `Condition<TSensory, TAction>` | Branches based on condition evaluation |

### Condition Evaluators

| Evaluator | Description |
|-----------|-------------|
| `ConditionEvaluator<TSensory>` | Base class for condition evaluators |
| `And<TSensory>` | Returns true if all conditions are true |
| `Or<TSensory>` | Returns true if any condition is true |
| `Not<TSensory>` | Inverts the condition result |
| `AfterSeconds<TSensory>` | Returns true after the most recent action has been called continuously for a specified duration |

## Node States

The State enum is now typically embedded in your TAction struct:

- `State.RUNNING` - Node is still executing
- `State.SUCCESS` - Node completed successfully
- `State.FAILURE` - Node failed

Your TAction struct should include a State field that control nodes can use to determine execution flow.

## Serialization

For detailed JSON grammar specification, see [JSON_spec.md](JSON_spec.md).

### Saving a Behavior Tree

```csharp
using BehaviorTree.Serializations;

string json = Serializer<GameInput, GameOutput>.WriteNodeJson(rootNode);
// Save json to file
```

### Loading a Behavior Tree

```csharp
using BehaviorTree.Serializations;

string json = // Load from file
Node<GameInput, GameOutput> rootNode = Deserializer<GameInput, GameOutput>.ReadNodeJson(json);
```

### Making Custom Nodes Serializable

1. Add `[SerializableNode("TypeName")]` attribute to node classes
2. Add `[SerializableEvaluator("TypeName")]` attribute to evaluator classes
3. Mark constructor parameters with `[ConstructorParameter("paramName")]` attribute

## Project Structure

```
├── BehaviorTree.cs          # Main BehaviorTree class
├── Nodes/
│   ├── Node.cs              # Base Node class
│   ├── Action.cs            # Base Action class
│   ├── Conditions/
│   │   ├── Condition.cs     # Condition node
│   │   ├── ConditionEvaluator.cs
│   │   ├── And.cs           # Logical AND
│   │   ├── Or.cs            # Logical OR
│   │   ├── Not.cs           # Logical NOT
│   │   └── Logics.cs        # Base Logic class
│   └── Controls/
│       ├── Sequence.cs      # Sequence node
│       ├── Selector.cs      # Selector node
│       └── Random.cs        # Random selection node
├── Serializations/
│   ├── Serializer.cs        # JSON serialization
│   ├── Deserializer.cs      # JSON deserialization
│   ├── ISerializableBT.cs   # Serializable interface
│   ├── Attributes/          # Serialization attributes
│   └── Exceptions/          # Custom exceptions
├── Sample/                  # Sample implementations
└── Testing/                 # Test scenes and scripts
```

## License

BSD 2-Clause License - see [LICENSE](LICENSE) for details.
