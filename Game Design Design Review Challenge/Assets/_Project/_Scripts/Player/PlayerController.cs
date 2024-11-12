using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    [SerializeField] InputProcessor input;
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float ClimbSpeed { get; private set; }
    [field: SerializeField] public float JumpSpeed { get; private set; }

    public Vector2 InputDirection { get; private set; }

    public bool IsGrounded { get; private set; }
    public int VineCount { get; private set; }
    StateMachine stateMachine;
    Rigidbody rb;

    void OnEnable() {
        input.OnMoveEvent += OnMove;
        GameManager.OnKeyCollected += DisableInput;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Vines")) VineCount++;
    }
    
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Vines")) VineCount--;
    }

    void OnDisable() {
        input.OnMoveEvent -= OnMove;
        GameManager.OnKeyCollected -= DisableInput;
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
        InitializeStateMachine();
    }

    void InitializeStateMachine() {
        stateMachine = new();

        MoveState move = new(this, input);
        ClimbingState climbing = new(this, input);
        JumpState jump = new(this, input);

        stateMachine.AddTransition(move, climbing, new FuncPredicate(() => move.InteractFlag));
        stateMachine.AddTransition(climbing, move, new FuncPredicate(() => climbing.InteractFlag));
        
        stateMachine.AddTransition(move, jump, new FuncPredicate(() => move.JumpFlag));
        stateMachine.AddTransition(climbing, jump, new FuncPredicate(() => climbing.JumpFlag));
        
        stateMachine.AddTransition(jump, move, new FuncPredicate(() => IsGrounded));
        stateMachine.AddTransition(jump, climbing, new FuncPredicate(() => jump.InteractFlag));
        stateMachine.SetState(move);
    }

    void Update() => stateMachine.Update();

    void FixedUpdate() {
        IsGrounded = Physics.Raycast(rb.position, Vector3.down, 1.6f);
        stateMachine.FixedUpdate();
    }

    void OnMove(Vector2 direction) => InputDirection = direction;
    void DisableInput() => input.Disable();
}