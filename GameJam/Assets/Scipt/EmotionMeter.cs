using UnityEngine;
using UnityEngine.UI;

public class EmotionMeter : MonoBehaviour
{
    public enum EmotionState
    {
        Low,
        Mid,
        High
    }

    public float minEmotion = 0f;
    public float maxEmotion = 100f;


    [Header("UI")]
    public Slider emotionSlider;
    public Image fillImage;
    public Image handleImage;

    [Header("Handle Sprites")]
    public Sprite highHandleSprite;
    public Sprite midHandleSprite;
    public Sprite lowHandleSprite;

    [Header("Emotion Colors")]
    public Color highColor = Color.yellow;
    public Color midColor = new Color(1f, 0.5f, 0f);
    public Color lowColor = Color.red;

    [Header("Tween Settings")]
    public float tweenTime = 0.25f;
    [Header("Tween Speed By Round")]
    public float startTweenTime = 0.35f;   // รอบแรก ช้า
    public float minTweenTime = 0.1f;      // เร็วสุด
    public int maxSpeedRound = 20;         // ครบกี่รอบถึงเร็วสุด

    float currentTweenTime;

    public EmotionState CurrentState { get; private set; }
    public float CurrentValue { get; private set; }

    public System.Action<EmotionState> OnEmotionStateChanged;

    void Start()
    {
        emotionSlider.minValue = minEmotion;
        emotionSlider.maxValue = maxEmotion;

        currentTweenTime = startTweenTime;

        SetEmotionValue(0, true);
    }


    public void IncreaseSpeedByRound(int currentRound)
    {
        float t = Mathf.Clamp01((float)currentRound / maxSpeedRound);
        currentTweenTime = Mathf.Lerp(startTweenTime, minTweenTime, t);
    }
    public void SetEmotionValue(int value, bool instant = false)
    {
        int clamped = Mathf.Clamp(value, 0, 100);
        CurrentValue = clamped;

        LeanTween.cancel(emotionSlider.gameObject);

        if (instant)
        {
            emotionSlider.value = clamped;
            ApplyVisual(clamped);
            return;
        }

        LeanTween.value(
            emotionSlider.gameObject,
            emotionSlider.value,
            clamped,
            currentTweenTime
        )
        .setOnUpdate((float v) =>
        {
            emotionSlider.value = v;
            ApplyVisual((int)v);  
        });
    }



    public void ApplyVisual(int realScore)
    {
        EmotionState newState = GetStateFromScore(realScore);

        if (newState != CurrentState)
        {
            CurrentState = newState;
            OnEmotionStateChanged?.Invoke(newState);
        }

        switch (newState)
        {
            case EmotionState.High:
                fillImage.color = highColor;
                handleImage.sprite = highHandleSprite;
                break;

            case EmotionState.Mid:
                fillImage.color = midColor;
                handleImage.sprite = midHandleSprite;
                break;

            case EmotionState.Low:
                fillImage.color = lowColor;
                handleImage.sprite = lowHandleSprite;
                break;
        }
    }

    EmotionState GetStateFromScore(int score)
    {
        if (score < 20)
            return EmotionState.Low;   // 😠 โกรธ
        else if (score < 60)
            return EmotionState.Mid;   // 😐 กลาง
        else
            return EmotionState.High;  // 🙂 ดี
    }



}
