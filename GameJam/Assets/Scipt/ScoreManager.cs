using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float survivedTime;
    public int correctHits;

    private float emotionSum;
    private int emotionSamples;

    public void RecordEmotion(float emotion)
    {
        emotionSum += emotion;
        emotionSamples++;
        survivedTime += Time.deltaTime;
    }

    public void RecordResult(bool hit)
    {
        if (hit)
            correctHits++;
    }

    public int CalculateFinalScore()
    {
        float avgEmotion =
            emotionSamples > 0 ? emotionSum / emotionSamples : 0f;

        float score =
            survivedTime * 10f +
            correctHits * 50f +
            avgEmotion * 5f;

        return Mathf.RoundToInt(score);
    }
    public int EvaluateResultIndex()
    {
        float avgEmotion =
            emotionSamples > 0 ? emotionSum / emotionSamples : 0f;

        if (avgEmotion >= 70f)
            return 2; // ดี
        else if (avgEmotion >= 40f)
            return 1; // ปานกลาง
        else
            return 0; // ไม่ดี
    }

}
