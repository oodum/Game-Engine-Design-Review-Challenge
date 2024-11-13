using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour {
    [SerializeField] ScoreSO score;

    float time;

    void Start() {
        time = 0;
    }

    void Update() {
        time += Time.deltaTime;
        score.Score = (int)time;
    }
}