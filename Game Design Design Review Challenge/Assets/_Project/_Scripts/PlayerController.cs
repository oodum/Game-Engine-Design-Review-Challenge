using System;
using Extensions;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] InputProcessor input;

    Rigidbody rb;
    Vector3 vel;

    bool isGrounded;


    void OnEnable() {
        input.OnMoveEvent += OnMove;
        input.OnJumpEvent += OnJump;
        input.OnInteractEvent += OnInteract;
    }

    void OnDisable() {
        input.OnMoveEvent -= OnMove;
        input.OnJumpEvent -= OnJump;
        input.OnInteractEvent -= OnInteract;
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        rb.linearVelocity = vel;
    }

    void OnMove(Vector2 direction) {
        vel = vel.With(x: direction.x);
    }

    void OnJump() {
        if (!isGrounded) return;
        vel = vel.Add(y: 5);
    }

    void OnInteract() {

    }
}
