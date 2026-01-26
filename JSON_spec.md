# Behavior Tree JSON Specification

This document describes the grammar and structure of serialized Behavior Tree JSON format used by SimpleUnityBehaviourTree.

## Overview

The serialization format represents behavior trees as nested JSON objects. Each node and condition evaluator is represented as an object with a `type` field identifying the class and a `params` field containing the constructor parameters.

## Grammar

### Top-Level Structure

A serialized behavior tree is a JSON object representing the root node:

```
BehaviorTree := Node
```

### Node Object

Every node (including the root) follows this structure:

```json
{
  "type": "<TypeName>",
  "params": {
    "<paramName1>": <value1>,
    "<paramName2>": <value2>,
    ...
  }
}
```

**Fields:**
- `type` (string, required): The type name specified in the `[SerializableNode]` attribute
- `params` (object, required): Object containing all constructor parameters

### Evaluator Object

Condition evaluators use the same structure as nodes:

```json
{
  "type": "<TypeName>",
  "params": {
    "<paramName1>": <value1>,
    "<paramName2>": <value2>,
    ...
  }
}
```

**Fields:**
- `type` (string, required): The type name specified in the `[SerializableEvaluator]` attribute
- `params` (object, required): Object containing all constructor parameters

### Parameter Values

Parameters can be:
- **Primitive types**: string, number, boolean
- **Arrays**: JSON arrays of any supported type
- **Nested Nodes**: Node objects (for child nodes)
- **Nested Evaluators**: Evaluator objects (for condition evaluators)

## Node Return Values

All nodes return a tuple `(bool, TAction)`:
- The `bool` indicates success (true if condition was met or action executed, false otherwise)
- The `TAction` contains the action to be executed

## Built-in Node Types

### Sequence

Executes children in a repeating pattern with special continuation logic:
- Executes the current child until the next child returns true
- Always executes the current child at least once per tick, even if the next is already true
- When reaching the last child, loops back to the first child

```json
{
  "type": "Sequence",
  "params": {
    "children": [<Node>, <Node>, ...]
  }
}
```

**Parameters:**
- `children` (array of Node): Child nodes to execute in looping sequence

**Behavior:**
- Maintains state of which child is currently executing
- Returns the action from the current child
- Advances to next child only when next child's condition becomes true

### Selector

Tries children in order until one returns true (succeeds).

```json
{
  "type": "Selector",
  "params": {
    "children": [<Node>, <Node>, ...]
  }
}
```

**Parameters:**
- `children` (array of Node): Child nodes to try in order

**Behavior:**
- Evaluates children from first to last
- Returns the first child that returns `(true, action)`
- If all children return false, returns `(false, default(TAction))`

### Random

Selects children based on weighted probability.

```json
{
  "type": "Random",
  "params": {
    "children": [<Node>, <Node>, ...],
    "weights": [<float>, <float>, ...]
  }
}
```

**Parameters:**
- `children` (array of Node): Child nodes to select from
- `weights` (array of float): Probability weights for each child (must match children array length)

### Condition

Executes a child node only if the condition evaluator returns true.

```json
{
  "type": "Condition",
  "params": {
    "evaluator": <Evaluator>,
    "childTrue": <Node>
  }
}
```

**Parameters:**
- `evaluator` (Evaluator): Condition evaluator to determine if child should execute
- `childTrue` (Node): Node to execute if evaluator returns true

**Behavior:**
- If evaluator returns true: executes and returns result from `childTrue`
- If evaluator returns false: returns `(false, default(TAction))`
- Typically used with Selector to try alternatives when condition fails

Action nodes always return `(true, action)` indicating successful execution.

```json
{
  "type": "<CustomActionName>",
  "params": {
    "<customParam1>": <value1>,
    "<customParam2>": <value2>,
    ...
  }
}
```

**Behavior:**
- Action nodes always return true (first element of tuple)
- The second element contains the action data structure to be executed ...
  }
Evaluators are not nodes - they are helper objects injected into Condition nodes to evaluate boolean conditions.
They implement `ISerializableBT` and return simple boolean values.

}
```

## Built-in Evaluator Types

### And

Returns true if all child conditions are true.

```json
{
  "type": "And",
  "params": {
    "conditions": [<Evaluator>, <Evaluator>, ...]
  }
}
```

**Parameters:**
- `conditions` (array of Evaluator): Child evaluators (all must be true)

### Or

Returns true if any child condition is true.

```json
{
  "type": "Or",
  "params": {
    "conditions": [<Evaluator>, <Evaluator>, ...]
  }
}
```

**Parameters:**
- `conditions` (array of Evaluator): Child evaluators (at least one must be true)

### Not

Inverts the child condition result.

```json
{
  "type": "Not",
  "params": {
    "condition": <Evaluator>
  }
}
```

**Parameters:**
- `condition` (Evaluator): Child evaluator to invert

## Custom Types

### Creating Custom Serializable Nodes

To make a custom node serializable:

1. Add the `[SerializableNode("TypeName")]` attribute to the class
2. Mark properties with `[ConstructorParameter("paramName")]` attribute
3. The parameter names in JSON must match constructor parameter names

Example:
```csharp
[SerializableNode("MoveToTarget")]
public class MoveToTarget<TSensory, TAction> : Action<TSensory, TAction>
    where TSensory : struct
    where TAction : struct
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
    
    // ... implementation
}
```

Serialized as:
```json
{
  "type": "MoveToTarget",
  "params": {
    "speed": 5.0,
    "targetPosition": {"x": 10, "y": 0, "z": 5}
  }
}
```

### Creating Custom Serializable Evaluators

Similar to custom nodes, but using `[SerializableEvaluator]` attribute:

```csharp
[SerializableEvaluator("IsHealthLow")]
public class IsHealthLow<TSensory> : ConditionEvaluator<TSensory>
    where TSensory : struct
{
    [ConstructorParameter("threshold")]
    public float threshold { get; private set; }
    
    public IsHealthLow(float threshold) : base("IsHealthLow")
    {
        this.threshold = threshold;
    }
    
    // ... implementation
}
```

Serialized as:
```json
{
  "type": "IsHealthLow",
  "params": {
    "threshold": 30.0
  }
}
```

## Complete Example
lector",
  "params": {
    "children": [
      {
        "type": "Condition",
        "params": {
          "evaluator": {
            "type": "And",
            "params": {
              "conditions": [
                {
                  "type": "SampleEvaluator",
                  "params": {
                    "p0": 10,
                    "p1": 1.5
                  }
                },
                {
                  "type": "SampleEvaluator",
                  "params": {
                    "p0": 20,
                    "p1": 2.5
                  }
                }
              ]
            }
          },
          "childTrue": {
            "type": "SampleAction",
            "params": {
              "p0": 1,
              "p1": 2.0
            }
          }
        }
      },
      {
        "type": "SampleAction",
        "params": {
          "p0": 2,
          "p1": 3.0
        }
      }
    ]
  }
}
```

This represents:
- A Selector with two children:
  1. A Condition that evaluates an AND of SampleEvaluator(10, 1.5) AND SampleEvaluator(20, 2.5)
     - If true: executes SampleAction(1, 2.0)
     - If false: returns (false, default) and Selector tries next child
  2. SampleAction(2, 3.0) - executed as fallback if Condition fails
- A Sequence with two children:
  1. A Condition that evaluates an AND of (NOT SampleEvaluator(10, 1.5)) AND SampleEvaluator(20, 2.5)
     - If true: executes SampleAction(1, 2.0)
     - If false: executes SampleAction(2, 3.0)
  2. SampleAction(0, 0.0)

## Serialization API

### Serializing

```csharp
using BehaviorTree.Serializations;

string json = Serializer<TSensory, TAction>.WriteNodeJson(rootNode);
```

### Deserializing

```csharp
using BehaviorTree.Serializations;

Node<TSensory, TAction> rootNode = Deserializer<TSensory, TAction>.ReadNodeJson(json);
```

## Notes

- All parameter names must match the constructor parameter names exactly
- The `type` field must match the name specified in `[SerializableNode]` or `[SerializableEvaluator]` attributes
- Arrays can contain nested nodes or evaluators
- The serialization is type-safe through generic TSensory and TAction type parameters
- Null values are serialized as JSON `null`
