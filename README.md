# SimpleUnityBehaviourTree

A lightweight and flexible Behavior Tree implementation for Unity, designed with simplicity and extensibility in mind.

## Features

- **Generic Agent Support**: Works with any agent type through generic type parameters
- **Struct-Based Input/Output**: Type-safe input and output structs for behavior tree execution
- **Core Node Types**: Includes essential behavior tree nodes (Sequence, Random, Condition, Action)
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
    public float deltaTime;
    public bool isPlayerNear;
    public float health;
}

// Define your output struct - result from node execution
public struct GameOutput
{
    public Node<object, GameInput, GameOutput>.State state;
    public string message;
    
    public static GameOutput Success() => new GameOutput 
    { 
        state = Node<object, GameInput, GameOutput>.State.SUCCESS 
    };
    
    public static GameOutput Running() => new GameOutput 
    { 
        state = Node<object, GameInput, GameOutput>.State.RUNNING 
    };
    
    public static GameOutput Failure() => new GameOutput 
    { 
        state = Node<object, GameInput, GameOutput>.State.FAILURE 
    };
}

// Create custom action
public class AttackAction : Action<object, GameInput, GameOutput>
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
var tree = new BehaviorTree<object, GameInput, GameOutput>("AI", root, null);

void Update()
{
    var input = new GameInput 
    { 
        deltaTime = Time.deltaTime,
        isPlayerNear = true,
        health = 100f
    };
    
    GameOutput output = tree.Tick(input);
    Debug.Log($"State: {output.state}, Message: {output.message}");
}
```

### Creating Custom Actions

Extend the `Action<Agent, TSensory, TAction>` class to create custom actions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableNode("MoveToTarget")]
public class MoveToTarget : Action<object, GameInput, GameOutput>
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

Extend `ConditionEvaluator<Agent, TSensory>` for custom conditions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableEvaluator("IsHealthLow")]
public class IsHealthLow : ConditionEvaluator<object, GameInput>
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
var shouldFlee = new And<object, GameInput>(new ConditionEvaluator<object, GameInput>[]
{
    isHealthLow,
    isEnemyNear
});

var shouldNotAttack = new Not<object, GameInput>(isEnemyNear);

var shouldHeal = new Or<object, GameInput>(new ConditionEvaluator<object, GameInput>[]
{
    isHealthLow,
    new IsPoisoned()
});

// Use in a Condition node - note that Condition nodes need custom implementation
// to handle state extraction from TAction
```

### Extending Control Nodes

When using struct-based I/O, control nodes like Sequence and Random need to know how to extract the state from TAction:

```csharp
public class CustomSequence : Sequence<object, GameInput, GameOutput>
{
    public CustomSequence(Node<object, GameInput, GameOutput>[] children)
        : base(children) { }

    protected override GameOutput CreateSuccessOutput(GameInput input)
    {
        return GameOutput.Success();
    }

    protected override GameOutput CreateRunningOutput(GameInput input)
    {
        return GameOutput.Running();
    }

    protected override Node<object, GameInput, GameOutput>.State GetStateFromOutput(GameOutput output)
    {
        return output.state;
    }
}
```

### Random Selection Node

The `Random` node selects children based on weighted probability. You'll need to extend it to handle struct-based I/O:

```csharp
public class CustomRandom : Random<object, GameInput, GameOutput>
{
    public CustomRandom(Node<object, GameInput, GameOutput>[] children, float[] weights)
        : base(children, weights) { }

    protected override GameOutput CreateFailureOutput(GameInput input)
    {
        return GameOutput.Failure();
    }

    protected override Node<object, GameInput, GameOutput>.State GetStateFromOutput(GameOutput output)
    {
        return output.state;
    }
}

// Usage
var randomNode = new CustomRandom(
    new Node<object, GameInput, GameOutput>[] 
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
| `Node<Agent, TSensory, TAction>` | Base abstract class for all nodes with struct-based I/O |
| `Action<Agent, TSensory, TAction>` | Base class for action/leaf nodes |
| `Sequence<Agent, TSensory, TAction>` | Executes children in order until one fails |
| `Random<Agent, TSensory, TAction>` | Selects children randomly based on weights |
| `Condition<Agent, TSensory, TAction>` | Branches based on condition evaluation |

### Condition Evaluators

| Evaluator | Description |
|-----------|-------------|
| `ConditionEvaluator<Agent, TSensory>` | Base class for condition evaluators |
| `And<Agent, TSensory>` | Returns true if all conditions are true |
| `Or<Agent, TSensory>` | Returns true if any condition is true |
| `Not<Agent, TSensory>` | Inverts the condition result |

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

string json = Serializer<object, GameInput, GameOutput>.WriteNodeJson(rootNode);
// Save json to file
```

### Loading a Behavior Tree

```csharp
using BehaviorTree.Serializations;

string json = // Load from file
Node<object, GameInput, GameOutput> rootNode = Deserializer<object, GameInput, GameOutput>.ReadNodeJson(json);
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
