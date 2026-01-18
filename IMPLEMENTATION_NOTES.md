# Behavior Tree Dynamic Re-evaluation Implementation

## Summary

This document describes the changes made to implement dynamic condition re-evaluation in the behavior tree system, as requested in the issue.

## Problem Statement (Japanese)

毎フレームのTickで全ての条件評価を行い、そこで適するアクションノードを毎フレーム選択して実行するように修正してください。
ただし、Sequenceノードはちゃんとフレームを越えて状態を保持するようにしてください。
具体的には、Sequenceノード内で、次の順番のノードが実行可能になるまで現在の順番のノードを実行し続けるようにしてください。どこかのフレームでSequenceノードの選択が外された場合は順番情報を消去してください。

## Translation

At every frame's Tick, perform all condition evaluations and select and execute the appropriate action node every frame based on those evaluations. However, ensure that Sequence nodes properly maintain state across frames. Specifically, within a Sequence node, continue executing the current child node until the next child node becomes executable. If the Sequence node becomes deselected at some frame, clear the sequence index information.

## Changes Made

### 1. Condition Node (`Nodes/Conditions/Condition.cs`)

**Before:**
- Evaluated condition only once (when `currentEvaluation == Evaluation.NOT_YET`)
- Cached the evaluation result
- Continued executing the same child until completion

**After:**
- Re-evaluates the condition every frame by calling `evaluator.Evaluate(agent)` on each Tick
- Detects when the evaluation changes (from TRUE to FALSE or vice versa)
- Resets the previous child when switching to ensure clean state transitions
- This allows dynamic switching between `childTrue` and `childFalse` based on real-time conditions

**Key Code Change:**
```csharp
// Re-evaluate condition every frame
bool evaluation = evaluator.Evaluate(agent);
Evaluation newEvaluation = evaluation ? Evaluation.TRUE : Evaluation.FALSE;

// If evaluation changed, reset the previous child
if (currentEvaluation != Evaluation.NOT_YET && currentEvaluation != newEvaluation)
{
    if (currentEvaluation == Evaluation.TRUE && childTrue != null)
    {
        childTrue.Reset();
    }
    else if (currentEvaluation == Evaluation.FALSE && childFalse != null)
    {
        childFalse.Reset();
    }
}
```

### 2. Sequence Node (`Nodes/Controls/Sequence.cs`)

**No changes required** - The existing implementation already:
- Maintains state across frames via the `childCurrent` index
- Executes the current child until it succeeds, then moves to the next child
- Resets the index when the sequence completes or fails
- Resets when `Reset()` is called by the parent (e.g., when deselected)

This satisfies all requirements from the problem statement.

### 3. Testing (`Testing/BehaviorTest.cs`, `Testing/TestRunner.cs`, `Testing/BehaviorTreeTest.csproj`)

Added comprehensive tests to verify the new behavior:

**Test 1: Condition Re-evaluation**
- Verifies that Condition nodes re-evaluate every frame
- Confirms that children switch dynamically when conditions change
- Validates that the previous child is reset when switching

**Test 2: Sequence State Maintenance**
- Verifies that Sequence nodes maintain their `childCurrent` index across frames
- Confirms that execution continues from the current child on subsequent ticks
- Validates proper progression through all children in sequence

**Test 3: Sequence Reset on Deselection**
- Verifies that when a Condition switches away from a Sequence child, the Sequence is reset
- Confirms that the `childCurrent` index is cleared (-1)
- Validates that the Sequence restarts from the beginning when re-selected

### 4. Build Configuration (`.gitignore`)

Added `.gitignore` to exclude build artifacts from version control.

## Behavior Examples

### Example 1: Dynamic Condition Switching

```
Frame 1: condition = false → executes actionFalse
Frame 2: condition = false → continues actionFalse  
Frame 3: condition = true  → resets actionFalse, starts actionTrue
Frame 4: condition = true  → continues actionTrue
```

### Example 2: Sequence State Maintenance

```
Frame 1: Sequence starts → childCurrent = 0, execute child[0]
Frame 2: child[0] still running → childCurrent = 0, continue child[0]
Frame 3: child[0] succeeds → childCurrent = 1, execute child[1]
Frame 4: child[1] still running → childCurrent = 1, continue child[1]
```

### Example 3: Sequence Reset on Deselection

```
Frame 1: Condition = true, Sequence starts → childCurrent = 0
Frame 2: Condition = true, Sequence continues → childCurrent = 0
Frame 3: Condition = false, switch to different child → Sequence.Reset() called, childCurrent = -1
Frame 4: Condition = true, Sequence restarts → childCurrent = 0 (starts from beginning)
```

## Impact

### Minimal Changes
- Only modified the `Condition.Tick()` method
- No changes to Sequence, Random, or other node types
- No breaking changes to the public API

### Benefits
- Enables reactive behavior trees that respond to changing conditions
- Maintains proper state management for sequential operations
- Ensures clean state transitions when switching between branches

### Compatibility
- Fully backward compatible with existing behavior trees
- Public API remains unchanged
- Serialization/deserialization still works as before

## Testing Results

All tests pass successfully:
```
✓ Condition re-evaluation test passed
✓ Sequence state maintenance test passed
✓ Sequence reset on deselection test passed
```

## Security Review

CodeQL analysis completed with no security issues found.
