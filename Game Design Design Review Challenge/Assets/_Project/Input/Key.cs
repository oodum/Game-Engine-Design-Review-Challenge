using System;
using UnityEngine;

public class Key : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.KeyCollected();
        }
    }
}