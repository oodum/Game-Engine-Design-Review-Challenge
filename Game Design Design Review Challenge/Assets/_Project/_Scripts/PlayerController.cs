using System;
using Extensions;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] InputProcessor input;
    [SerializeField] float moveSpeed, climbSpeed, jumpSpeed;

    Rigidbody rb;
    Vector3 vel;

    bool isGrounded = true;

    void OnEnable() {
        input.OnMoveEvent += OnMove;
        input.OnJumpEvent += OnJump;
        input.OnInteractEvent += OnInteract;

        GameManager.OnKeyCollected += DisableInput;
    }

    void OnDisable() {
        input.OnMoveEvent -= OnMove;
        input.OnJumpEvent -= OnJump;
        input.OnInteractEvent -= OnInteract;

        GameManager.OnKeyCollected -= DisableInput;
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        rb.linearVelocity = rb.linearVelocity.With(x: vel.x, z: vel.z);
        CheckGround();
    }

    void OnMove(Vector2 direction) {
        vel = vel.With(x: direction.x * moveSpeed);
    }

    void CheckGround() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void OnJump() {
        if (!isGrounded) return;
        rb.linearVelocity = rb.linearVelocity.Add(y: jumpSpeed);
        print("yid");
    }

    void OnInteract() { }

    void DisableInput() => input.Disable();
}
