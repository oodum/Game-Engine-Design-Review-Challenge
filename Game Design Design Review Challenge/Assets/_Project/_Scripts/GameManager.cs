using System;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public static event Action OnKeyCollected = delegate { };

    public void KeyCollected() { OnKeyCollected.Invoke(); }

    public void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
}