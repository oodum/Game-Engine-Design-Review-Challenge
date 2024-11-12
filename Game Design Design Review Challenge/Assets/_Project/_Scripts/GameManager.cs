using System;
public class GameManager : Singleton<GameManager> {
    public static event Action OnKeyCollected = delegate { };

    public void KeyCollected() {
        OnKeyCollected.Invoke();
    }
}
