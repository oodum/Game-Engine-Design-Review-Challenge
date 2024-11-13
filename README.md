# Game Engine Design Review Challenge
## Adam Tam & Arianna Thorson

## How to Play
- A and D to move
- Space to jump
- E to interact
- W and S to climb (only when attached to vines)
Goal: Collect the key to win!
Video: https://youtu.be/nODMlEqYerg 
## Pattern Implementations
### Singleton
The Singleton pattern was implemented for the GameManager. This uses a Singleton<T> class:
![firefox_4XWlFGMsUX.png](img%2Ffirefox_4XWlFGMsUX.png)
```csharp
public class Singleton<T> : MonoBehaviour where T : Component {
    static T instance;
    public static bool Exists => instance != null;

    public static T Instance {
        get {
            if (instance == null) {
                // try to find the instance in the scene
                instance = FindAnyObjectByType<T>();
                if (instance == null) {
                    // if still not found, create a new game object and add the component
                    var go = new GameObject(typeof(T).Name + " (Singleton)");
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// If you need to use Awake, override it and call base.Awake()
    /// </summary>
    protected virtual void Awake() {
        if (!Application.isPlaying) return;
        instance = this as T;
    }

    /// <summary>
    /// If you need to use OnDestroy, override it and call base.OnDestroy()
    /// </summary>
    protected virtual void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}
```
The GameManager class is used by the Key when collided to invoke the GameOver/KeyCollect event, and the GameUI is listening so it can update the UI, which also contains a button that asks the GameManager to restart the scene.

### Observer
The Observer pattern was implemented in a lot of places, but I'd like to showcase the input.![firefox_xs1nThcf44.png](img%2Ffirefox_xs1nThcf44.png)

The InputProcessor class is a ScriptableObject that listens to the input provided by Unity's Input System, and acts as a facade to the input system. By this, I mean that the input is processed by the InputProcessor and it then provides a simple way for objects to listen in to those events through events instead of having to deal with the input system directly.
Note that the InputProcessor inherits from IPlayerActions, which means that it can be provided callbacks right from the input system.
```csharp
public class InputProcessor : ScriptableObject, PlayerInputActions.IPlayerActions {
    public event Action<Vector2> OnMoveEvent = delegate { };
    public event Action OnInteractEvent = delegate { };
    public event Action OnJumpEvent = delegate { };


    PlayerInputActions inputActions;

    void OnEnable() {
        inputActions = new();
        inputActions.Player.SetCallbacks(this);
        inputActions.Player.Enable();
    }

    void OnDisable() { Disable(); }

    public void Enable() { inputActions.Player.Enable(); }

    public void Disable() { inputActions.Player.Disable(); }

    public void OnMove(InputAction.CallbackContext context) { OnMoveEvent.Invoke(context.ReadValue<Vector2>()); }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            OnInteractEvent.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            OnJumpEvent.Invoke();
        }
    }
}
```

Note: Arianna made a UML diagram for this as well (below), but it wasn't completely correct with the arrows. Please refer to the one above.
![firefox_NAiQOOxaHe.png](img%2Ffirefox_NAiQOOxaHe.png)

### State
The State pattern was implemented in the player. This allows the states to contain their own logic so the PlayerController itself doesn't have to perform tons of checks to see what state it's in and what to do. 

The state machine contains most of the logic, and deals with changing states and updating the current state. The states themselves are just classes that implement the IState interface, and contain the logic for what to do when they start, update, fixed update, and exit. 
The changing of states is handled by the IPredicate interface, which is implemented by the FuncPredicate and AlwaysTruePredicate classes. The FuncPredicate takes a function that returns a boolean, and the AlwaysTruePredicate always returns true. 
```csharp
public class StateMachine {
    StateNode current;
    readonly Dictionary<Type, StateNode> nodes = new();
    readonly HashSet<ITransition> anyTransitions = new();

    public void ChangeState(IState state) {
        if (state == current.State) return;
        current.State?.Exit();
        current = nodes[state.GetType()];
        current.State.Start();
    }

    public void SetState(IState state) {
        current = nodes[state.GetType()];
        current.State.Start();
    }

    public void Update() {
        var transition = GetTransition();
        if (transition != null) ChangeState(transition.To);
        current.State?.Update();
    }
    
    public void FixedUpdate() {
        current.State?.FixedUpdate();
    }

    ITransition GetTransition() {
        foreach (var transition in anyTransitions)
            if (transition.Predicate.Evaluate())
                return transition;
        foreach (var transition in current.Transitions)
            if (transition.Predicate.Evaluate())
                return transition;
        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate predicate) => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, predicate);
    public void AddAnyTransition(IState to, IPredicate predicate) => anyTransitions.Add(new Transition(GetOrAddNode(to).State, predicate));

    StateNode GetOrAddNode(IState state) {
        if (nodes.TryGetValue(state.GetType(), out var node)) return node;
        node = new(state);
        nodes.Add(state.GetType(), node);
        return node;
    }

    class StateNode {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state) {
            State = state;
            Transitions = new();
        }

        public void AddTransition(IState to, IPredicate predicate) => Transitions.Add(new Transition(to, predicate));
    }
}


public interface IState {
    public void Start();
    public void Update();
    public void FixedUpdate();
    public void Exit();
}

public abstract class PlayerBaseState : IState {
    protected PlayerController player;
    protected InputProcessor input;

    public PlayerBaseState(PlayerController player, InputProcessor input) {
        this.player = player;
        this.input = input;
    }

    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}

public interface IPredicate {
    bool Evaluate();
}

public class FuncPredicate : IPredicate {
    readonly Func<bool> predicate;
    public FuncPredicate(Func<bool> predicate) => this.predicate = predicate;
    public bool Evaluate() => predicate.Invoke();
}

public class AlwaysTruePredicate : IPredicate {
    public bool Evaluate() => true;
}

public interface ITransition {
    IState To { get; }
    IPredicate Predicate { get; }
}

public class Transition : ITransition {
    public IState To { get; }
    public IPredicate Predicate { get; }

    public Transition(IState to, IPredicate predicate) {
        To = to;
        Predicate = predicate;
    }
}
```

This is how it's implemented in the Player:
```csharp
 void InitializeStateMachine() {
        stateMachine = new();

        MoveState move = new(this, input);
        ClimbingState climbing = new(this, input);
        JumpState jump = new(this, input);
        DieState die = new(this, input, spawnPosition);

        stateMachine.AddTransition(move, climbing, new FuncPredicate(() => move.InteractFlag));
        stateMachine.AddTransition(climbing, move, new FuncPredicate(() => climbing.InteractFlag));
        
        stateMachine.AddTransition(move, jump, new FuncPredicate(() => move.JumpFlag));
        stateMachine.AddTransition(climbing, jump, new FuncPredicate(() => climbing.JumpFlag));
        
        stateMachine.AddTransition(jump, move, new FuncPredicate(() => IsGrounded));
        stateMachine.AddTransition(jump, climbing, new FuncPredicate(() => jump.InteractFlag));
        
        stateMachine.AddTransition(die, move, new AlwaysTruePredicate());
        
        stateMachine.AddAnyTransition(die, new FuncPredicate(() => rb.position.y < -11));
        
        stateMachine.SetState(move);
    }
```

![firefox_1265eYIOpi.png](img%2Ffirefox_1265eYIOpi.png)


## DLL
Since I'm using Linux, I couldn't get the DLL to work properly, but I still was able to create it. The DLL's job is to take an int score, and prepend it with zeroes to match how the arcades do it:
123 -> 00000123
```c++
// Score.h
#pragma once
#ifndef __SCORE__
#define __SCORE__
struct Score {
	int score;
	int length;
};
#endif

// ScoreString.cpp
#include "ScoreString.h"
#include <string>

ScoreString::ScoreString() {
	SetScore();
}

Score ScoreString::GetScore() const {
	return score;
}

void ScoreString::SetScore(int score) {
	this->score.score = score;
}

void ScoreString::SetLength(int length) {
	this->score.length = length;
}

std::string ScoreString::ToString() const {
	auto scoreString = std::to_string(score.score);
	if (score.length == 0) {
		return scoreString;
	}
	else {
		auto scoreLength = scoreString.length();
		if (scoreLength < score.length) {
			auto padding = std::string(score.length - scoreLength, '0');
			return padding + scoreString;
		}
		else {
			return scoreString;
		}
	}
}
```

The rest of the code, as well as the DLL, can be found in the DK_DLL folder.

Because I could not implement it in Unity, I was able to make a native version in C# that does the same thing, which can be viewed either in the Build, or in the gameplay video. 
However, here is an example of it working in the console:
![VS_SwKEfNBbcE.png](img%2FVS_SwKEfNBbcE.png)

## Contributions
Adam (90%):
- 3 Design patterns
- DLL
- README
- All UML diagrams
- Video

Arianna (10%):
- 1 UML diagram (was replaced)