public static class ScoreString {
    public static string GetScoreString(int score, int length) {
        string scoreString = score.ToString();
        if (scoreString.Length < length) {
            scoreString = scoreString.PadLeft(length, '0');
        }
        return scoreString;
    }
}