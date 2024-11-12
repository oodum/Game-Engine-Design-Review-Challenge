using Extensions;
using UnityEngine;

public class MoveState : PlayerBaseState {
    Rigidbody rb;
    public bool InteractFlag { get; private set; }
    public bool JumpFlag { get; private set; }

    public MoveState(PlayerController player, InputProcessor input) : base(player, input) { rb = player.GetComponent<Rigidbody>(); }

    public override void Start() {
        InteractFlag = false;
        JumpFlag = false;
        input.OnInteractEvent += TriggerInteract;
        input.OnJumpEvent += TriggerJump;
    }

    public override void FixedUpdate() { rb.linearVelocity = rb.linearVelocity.With(x: player.InputDirection.x * player.MoveSpeed); }

    public override void Exit() {
        input.OnInteractEvent -= TriggerInteract;
        input.OnJumpEvent -= TriggerJump;
    }

    void TriggerInteract() {
        if (player.VineCount > 0) InteractFlag = true;
    }

    void TriggerJump() {
        if (player.IsGrounded) JumpFlag = true;
    }
}

public class ClimbingState : PlayerBaseState {
    Rigidbody rb;
    public bool InteractFlag { get; private set; }
    public bool JumpFlag { get; private set; }
    public ClimbingState(PlayerController player, InputProcessor input) : base(player, input) { rb = player.GetComponent<Rigidbody>(); }

    public override void Start() {
        InteractFlag = false;
        JumpFlag = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        input.OnInteractEvent += TriggerInteract;
        input.OnJumpEvent += TriggerJump;
    }

    public override void FixedUpdate() { rb.linearVelocity = rb.linearVelocity.With(y: player.InputDirection.y * player.ClimbSpeed * player.VineCount); }

    public override void Exit() {
        input.OnInteractEvent -= TriggerInteract; 
        input.OnJumpEvent -= TriggerJump;
        rb.useGravity = true;
    }

    void TriggerInteract() { InteractFlag = true; }
    void TriggerJump() { JumpFlag = true; }
}

public class JumpState : PlayerBaseState {
    Rigidbody rb;
    public bool InteractFlag { get; private set; }
    public JumpState(PlayerController player, InputProcessor input) : base(player, input) { rb = player.GetComponent<Rigidbody>(); }

    public override void Start() {
        rb.linearVelocity = rb.linearVelocity.With(y: player.JumpSpeed);
        input.OnInteractEvent += TriggerInteract;
        InteractFlag = false;
    }

    public override void FixedUpdate() {
        rb.linearVelocity = rb.linearVelocity.With(x: player.InputDirection.x * player.MoveSpeed);
    }
    
    public override void Exit() {
        input.OnInteractEvent -= TriggerInteract;
    }
    
    void TriggerInteract() {
        if (player.VineCount > 0) InteractFlag = true;
    }
}