using UnityEngine;

[CreateAssetMenu(menuName = "Create ScoreSO", fileName = "ScoreSO", order = 0)]
// ReSharper disable once InconsistentNaming
public class ScoreSO : ScriptableObject {
    public int Score;
    public int Length;
    
    public string String => ScoreString.GetScoreString(Score, Length);

    public override string ToString() => ScoreString.GetScoreString(Score, Length);
}