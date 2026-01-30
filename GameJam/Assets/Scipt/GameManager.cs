using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Time")]
    public float gameDuration = 60f;
    public TMP_Text timeText; 

    [Header("Input")]
    public KeyCode inputKey = KeyCode.Space;

    [Header("References")]
    public RhythmBarController rhythm;
    public EmotionMeter emotion;
    public ScoreManager score;

    private float currentTime;
    private bool isGameOver;
    public EndGameUIController endGameUI;

    void Start()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
        currentTime = gameDuration;
        emotion.OnEmotionEmpty += GameOverByEmotion;
        UpdateTimeText();
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateTimer();
        UpdateDifficultyByTime();
        HandleInput();

        score.RecordEmotion(emotion.CurrentEmotion);
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateTimeText();

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    void UpdateTimeText()
    {
        if (timeText != null)
            timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    void UpdateDifficultyByTime()
    {
        // 0 = เริ่มเกม, 1 = เวลาหมด
        float timeProgress = 1f - (currentTime / gameDuration);
        rhythm.UpdateRhythm(timeProgress);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(inputKey))
        {
            bool hit = rhythm.CheckHit();

            emotion.ApplyResult(hit);
            score.RecordResult(hit);

            // ⭐ เปลี่ยนขนาด Red Zone แบบทันที
            float timeProgress = 1f - (currentTime / gameDuration);
            rhythm.ApplyDifficultyStep(timeProgress);
        }
    }
    void EndGame()
    {
        isGameOver = true;

        int resultIndex = score.EvaluateResultIndex();
        endGameUI.ShowResult(resultIndex);
    }
    void GameOverByEmotion()
    {
        if (isGameOver) return;

        isGameOver = true;
        currentTime = 0;

        int resultIndex = score.EvaluateResultIndex();
        endGameUI.ShowResult(resultIndex);
    }

}
