using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project._Scripts {
    public class GameUI : MonoBehaviour {
        UIDocument document;
        [SerializeField] VisualTreeAsset winScreen;

        void Awake() { document = GetComponent<UIDocument>(); }

        void OnEnable() { GameManager.OnKeyCollected += GameOver; }

        void OnDisable() { GameManager.OnKeyCollected -= GameOver; }

        void GameOver() {
            document.visualTreeAsset = winScreen;
            var root = document.rootVisualElement;
            root.Q<Button>("QuitButton").clicked += () => Application.Quit();
            root.Q<Button>("PlayAgainButton").clicked += () => GameManager.Instance.RestartGame();
        }
    }
}