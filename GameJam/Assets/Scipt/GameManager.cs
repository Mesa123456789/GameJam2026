using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public EndGameUIController endGameUI;

    private float currentTime;
    private bool isGameOver;
    public SceneEmotionCharacterController characterController;
    public RectTransform playerCharacter;
    public RectTransform bossCharacter;
    [Header("Character Bounce")]
    public float bounceHeight = 30f;   // pixel
    public float bounceTime = 0.2f;

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

    void Start()
    {

        Cursor.visible = false;
        Time.timeScale = 1f;
        AudioManager.Instance.ApplyAll();

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

        UpdateMusicByEmotion(); 
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

            // ⭐ เด้งตัวละคร
            BounceCharacter(playerCharacter);
            BounceCharacter(bossCharacter);

            float timeProgress = 1f - (currentTime / gameDuration);
            rhythm.ApplyDifficultyStep(timeProgress);
        }
    }


    void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        emotion.FreezeEmotion();
        float finalEmotion =
            emotion.GetCurrentEmotionValue(); // slider.value

        int resultIndex =
            score.EvaluateResultIndex(finalEmotion);

        string sceneName =
            SceneManager.GetActiveScene().name;

        ProgressManager.Instance
            .SaveSceneResult(sceneName, resultIndex);

        endGameUI.ShowResult(resultIndex);
    }

    void GameOverByEmotion()
    {
        if (isGameOver) return;

        isGameOver = true;
        currentTime = 0;

      
        emotion.FreezeEmotion();

        float finalEmotionValue =
            emotion.GetCurrentEmotionValue();

        int resultIndex =
            score.EvaluateResultIndex(finalEmotionValue);

        endGameUI.ShowResult(resultIndex);
    }
    void UpdateMusicByEmotion()
    {
        //float finalEmotionValue =
        //    emotion.GetCurrentEmotionValue();

        //if (finalEmotionValue > 60f)
        //    AudioManager.Instance.PlayCalm();
        //else if (finalEmotionValue > 20f)
        //    AudioManager.Instance.PlayMid();
        //else
        //    AudioManager.Instance.PlayChaos();
    }

}
