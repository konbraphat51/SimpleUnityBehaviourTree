# SimpleUnityBehaviourTree

A lightweight and flexible Behavior Tree implementation for Unity, designed with simplicity and extensibility in mind.

## Features

- **Generic Agent Support**: Works with any agent type through generic type parameters
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

### Creating a Simple Behavior Tree

```csharp
using BehaviorTree;
using BehaviorTree.Nodes;

// Define your agent type
public class Enemy
{
    public float health;
    public Vector3 position;
}

// Create nodes
var attackAction = new MyAttackAction();
var patrolAction = new MyPatrolAction();

// Build the tree
var sequence = new Sequence<Enemy>(new Node<Enemy>[] 
{
    attackAction,
    patrolAction
});

// Create the behavior tree
var tree = new BehaviorTree<Enemy>("EnemyAI", sequence, myEnemy);

// Run the tree each frame
void Update()
{
    tree.Tick();
}
```

### Creating Custom Actions

Extend the `Action<Agent>` class to create custom actions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableNode("MoveToTarget")]
public class MoveToTarget<Agent> : Action<Agent> where Agent : IMovable
{
    [ConstructorParameter("speed")]
    public float speed { get; private set; }

    [ConstructorParameter("targetPosition")]
    public Vector3 targetPosition { get; private set; }

    public MoveToTarget(float speed, Vector3 targetPosition) : base("MoveToTarget")
    {
        this.speed = speed;
        this.targetPosition = targetPosition;
    }

    protected override bool TakeAction(Agent agent)
    {
        // Return true when action is complete, false to keep running
        return agent.MoveTowards(targetPosition, speed);
    }
}
```

### Creating Custom Condition Evaluators

Extend `ConditionEvaluator<Agent>` for custom conditions:

```csharp
using BehaviorTree.Nodes;
using BehaviorTree.Serializations;

[SerializableEvaluator("IsHealthLow")]
public class IsHealthLow<Agent> : ConditionEvaluator<Agent> where Agent : IHasHealth
{
    [ConstructorParameter("threshold")]
    public float threshold { get; private set; }

    public IsHealthLow(float threshold) : base("IsHealthLow")
    {
        this.threshold = threshold;
    }

    public override bool Evaluate(Agent agent)
    {
        return agent.Health < threshold;
    }
}
```

### Using Conditions with Logic Operators

```csharp
// Create condition evaluators
var isHealthLow = new IsHealthLow<Enemy>(30f);
var isEnemyNear = new IsEnemyNear<Enemy>(10f);

// Combine with logic operators
var shouldFlee = new And<Enemy>(new ConditionEvaluator<Enemy>[]
{
    isHealthLow,
    isEnemyNear
});

var shouldNotAttack = new Not<Enemy>(isEnemyNear);

var shouldHeal = new Or<Enemy>(new ConditionEvaluator<Enemy>[]
{
    isHealthLow,
    new IsPoisoned<Enemy>()
});

// Use in a Condition node
var condition = new Condition<Enemy>(
    shouldFlee,
    new FleeAction<Enemy>(),  // Execute if true
    new AttackAction<Enemy>() // Execute if false
);
```

### Random Selection Node

The `Random` node selects children based on weighted probability:

```csharp
var randomNode = new Random<Enemy>(
    new Node<Enemy>[] 
    {
        new PatrolAction<Enemy>(),
        new IdleAction<Enemy>(),
        new WanderAction<Enemy>()
    },
    new float[] { 0.5f, 0.3f, 0.2f } // Weights
);
```

## Node Types

### Core Nodes

| Node | Description |
|------|-------------|
| `Node<Agent>` | Base abstract class for all nodes |
| `Action<Agent>` | Base class for action/leaf nodes |
| `Sequence<Agent>` | Executes children in order until one fails |
| `Random<Agent>` | Selects children randomly based on weights |
| `Condition<Agent>` | Branches based on condition evaluation |

### Condition Evaluators

| Evaluator | Description |
|-----------|-------------|
| `ConditionEvaluator<Agent>` | Base class for condition evaluators |
| `And<Agent>` | Returns true if all conditions are true |
| `Or<Agent>` | Returns true if any condition is true |
| `Not<Agent>` | Inverts the condition result |

## Node States

Each node returns one of three states:

- `State.RUNNING` - Node is still executing
- `State.SUCCESS` - Node completed successfully
- `State.FAILURE` - Node failed

## Serialization

### Saving a Behavior Tree

```csharp
using BehaviorTree.Serializations;

string json = Serializer<Enemy>.WriteNodeJson(rootNode);
// Save json to file
```

### Loading a Behavior Tree

```csharp
using BehaviorTree.Serializations;

string json = // Load from file
Node<Enemy> rootNode = Deserializer<Enemy>.ReadNodeJson(json);
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
