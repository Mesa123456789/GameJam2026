using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Time")]
    public float gameDuration = 30f;
    public TMP_Text timeText;

    [Header("Input")]
    public KeyCode inputKey = KeyCode.Space;

    [Header("References")]
    public RhythmBarController rhythm;
    public ScoreManager score;
    public EndGameUIController endGameUI;

    [Header("Character Bounce")]
    public RectTransform playerCharacter;
    public RectTransform bossCharacter;
    public float bounceHeight = 30f;
    public float bounceTime = 0.2f;
    public EmotionMeter emotionMeter;

    private float currentTime;
    private bool isGameOver;
    int roundCount;

    [Header("Input Sound")]
    public AudioSource inputAudio;
    public AudioClip hitClip;    // เสียงกดดี (Smile)
    public AudioClip missClip;   // เสียงกดพลาด (Neutral / Angry)
    public EmotionAudioController emotionAudio;

    void Start()
    {
        Cursor.visible = false;
        //Time.timeScale = 1f;

        // ✅ เก็บ AudioManager
        AudioManager.Instance.ApplyAll();

        currentTime = gameDuration;
        UpdateTimeText();

        if (score != null)
            score.ResetScore();
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateTimer();
        UpdateDifficultyByTime();
        HandleInput();
    }

    // ⏱ เวลาเกม
    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateTimeText();

        if (currentTime <= 0)
            EndGame();
    }

    void UpdateTimeText()
    {
        if (timeText != null)
            timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // 🎚 เร่งความเร็ว indicator ตามเวลา
    void UpdateDifficultyByTime()
    {
        float timeProgress = 1f - (currentTime / gameDuration);
        rhythm.UpdateSpeedByTime(timeProgress);
    }
    void HandleInput()
    {
        if (!Input.GetKeyDown(inputKey)) return;

        roundCount++;

        int index = rhythm.GetCurrentIndex();
        RhythmBarController.EmotionType emotion =
            rhythm.GetCurrentEmotion();
        // หลังจากรู้ emotion แล้ว
        if (emotion == RhythmBarController.EmotionType.Smile)
        {
            rhythm.PlayHitFeedback();   // 🟢 กดโดน
        }
        else
        {
            rhythm.PlayMissFeedback();  // 🔴 กดพลาด
        }

        int scoreDelta = GetScoreFromEmotion(emotion);

        Debug.Log(
            $"[INPUT] Slot:{index} | Emotion:{emotion} | Delta:{scoreDelta}"
        );

        // 🔊 เล่นเสียงตามผลลัพธ์การกด
        PlayInputSound(emotion);

        score.AddScore(scoreDelta);

        Debug.Log(
            $"[SCORE] TotalScore = {score.TotalScore}"
        );

        emotionMeter.IncreaseSpeedByRound(roundCount);

        rhythm.RandomizeSlots();

        BounceCharacter(playerCharacter);
        BounceCharacter(bossCharacter);
    }

    void PlayInputSound(RhythmBarController.EmotionType emotion)
    {
        if (inputAudio == null) return;

        switch (emotion)
        {
            case RhythmBarController.EmotionType.Smile:
                if (hitClip != null)
                    inputAudio.PlayOneShot(hitClip);
                break;

            case RhythmBarController.EmotionType.Neutral:
            case RhythmBarController.EmotionType.Angry:
                if (missClip != null)
                    inputAudio.PlayOneShot(missClip);
                break;
        }
    }

    int GetScoreFromEmotion(RhythmBarController.EmotionType emotion)
    {
        switch (emotion)
        {
            case RhythmBarController.EmotionType.Smile:
                return 10;
            case RhythmBarController.EmotionType.Neutral:
                return -10;
            case RhythmBarController.EmotionType.Angry:
                return -15;
        }
        return 0;
    }
    void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        rhythm.StopRhythm();

        // 🔇 ปิดเสียง emotion
        if (emotionAudio != null)
            emotionAudio.StopEmotionAudio();

        int resultIndex = score.EvaluateResultIndex();

        string sceneName =
            SceneManager.GetActiveScene().name;

        ProgressManager.Instance
            .SaveSceneResult(sceneName, resultIndex);

        endGameUI.ShowResult(resultIndex);
    }

    void BounceCharacter(RectTransform target)
    {
        if (target == null) return;

        LeanTween.cancel(target.gameObject);

        Vector2 basePos = target.anchoredPosition;

        LeanTween.moveY(
            target,
            basePos.y + bounceHeight,
            bounceTime * 0.5f
        )
        .setEaseOutQuad()
        .setIgnoreTimeScale(true)
        .setOnComplete(() =>
        {
            LeanTween.moveY(
                target,
                basePos.y,
                bounceTime * 0.5f
            )
            .setEaseInQuad()
            .setIgnoreTimeScale(true);
        });
    }
}
