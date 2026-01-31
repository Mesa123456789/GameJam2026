using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score")]
    public int TotalScore { get; private set; }

    [Header("Emotion Thresholds")]
    public int midThreshold = 0;     // < 0 = Low
    public int highThreshold = 60;   // >= 60 = High

    [Header("Reference")]
    public EmotionMeter emotionMeter;

    public void AddScore(int delta)
    {
        TotalScore += delta;
        TotalScore = Mathf.Clamp(TotalScore, 0, 100);

        emotionMeter.SetEmotionValue(TotalScore);
    }

    public int EvaluateResultIndex()
    {
        if (TotalScore < 20)
            return 0;
        else if (TotalScore < 60)
            return 1; 
        else
            return 2; 
    }
    public void ResetScore()
    {
        TotalScore = 0;
        emotionMeter.SetEmotionValue(0, true);

 
        emotionMeter.ApplyVisual(0);
    }

}
