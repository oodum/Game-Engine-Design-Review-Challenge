using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using Random = UnityEngine.Random;

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