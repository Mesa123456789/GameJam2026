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
    [Header("Idle Penalty")]
    public float idleLimit = 2.0f;      // ไม่กดเกินกี่วินาที
    public int idlePenalty = -5;        // โดนหักกี่คะแนน

    float idleTimer;
    Vector2 playerBasePos;
    Vector2 bossBasePos;
    bool basePosInitialized;

    [Header("Input Sound")]
    public AudioSource inputAudio;
    public AudioClip hitClip;    // เสียงกดดี (Smile)
    public AudioClip missClip;   // เสียงกดพลาด (Neutral / Angry)
    public EmotionAudioController emotionAudio;
    float lastInputSoundTime;
    public float inputSoundCooldown = 0.05f;
    [Header("Idle Warning Shake")]
    public RectTransform warningTarget;   // เช่น indicator
    public float warningShakeAmount = 12f;
    public float warningShakeTime = 0.15f;

    bool hasPlayedIdleWarning;

    void PlayInputSound(RhythmBarController.EmotionType emotion)
    {
        if (inputAudio == null) return;
        if (Time.time - lastInputSoundTime < inputSoundCooldown) return;

        lastInputSoundTime = Time.time;

        if (emotion == RhythmBarController.EmotionType.Smile)
            inputAudio.PlayOneShot(hitClip);
        else
            inputAudio.PlayOneShot(missClip);
    }
    void Start()
    {
        Cursor.visible = false;
        AudioManager.Instance.ApplyAll();

        currentTime = gameDuration;
        UpdateTimeText();

        if (score != null)
            score.ResetScore();

        // ⭐ เก็บตำแหน่งฐาน
        if (playerCharacter != null)
            playerBasePos = playerCharacter.anchoredPosition;

        if (bossCharacter != null)
            bossBasePos = bossCharacter.anchoredPosition;

        basePosInitialized = true;
    }


    void Update()
    {
        if (isGameOver) return;

        UpdateTimer();
        UpdateDifficultyByTime();
        UpdateIdlePenalty();   // ⭐ เพิ่มตรงนี้
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
        idleTimer = 0f;
        hasPlayedIdleWarning = false; // ⭐ รีเซ็ตเตือน

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

    void UpdateIdlePenalty()
    {
        idleTimer += Time.deltaTime;

        // ⚠️ เตือนก่อนโดนหัก
        if (!hasPlayedIdleWarning && idleTimer > idleLimit * 0.8f)
        {
            hasPlayedIdleWarning = true;
            PlayIdleWarningShake();
        }

        // 🔻 โดนหักคะแนนจริง
        if (idleTimer >= idleLimit)
        {
            idleTimer = 0f;
            hasPlayedIdleWarning = false;

            score.AddScore(idlePenalty);

            Debug.Log("[IDLE] No input too long → penalty");

            rhythm.PlayMissFeedback();
        }
    }

    void PlayIdleWarningShake()
    {
        if (warningTarget == null) return;

        LeanTween.cancel(warningTarget.gameObject);

        Vector3 basePos = warningTarget.localPosition;

        LeanTween.moveLocalX(
            warningTarget.gameObject,
            basePos.x + warningShakeAmount,
            warningShakeTime * 0.5f
        )
        .setEaseShake()
        .setLoopPingPong(1)
        .setOnComplete(() =>
        {
            warningTarget.localPosition = basePos;
        });
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
        if (target == null || !basePosInitialized) return;

        Vector2 basePos =
            (target == playerCharacter) ? playerBasePos : bossBasePos;

        LeanTween.cancel(target.gameObject);
        target.anchoredPosition = basePos; // ⭐ รีเซ็ตก่อนเด้งเสมอ

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
