using UnityEngine;
using UnityEngine.UI;

public class EmotionMeter : MonoBehaviour
{
    [Header("Emotion Settings")]
    public float maxEmotion = 100f;
    public float drainPerSecond = 5f;
    public float gainOnHit = 10f;
    public float lossOnMiss = 15f;

    [Header("UI")]
    public Slider emotionSlider;

    public float CurrentEmotion { get; private set; }
    [Header("Tween Settings")]
    public float upTweenTime = 0.15f;
    public float downTweenTime = 0.4f;

    [Header("Emotion Colors")]
    public Color highColor = Color.yellow;
    public Color midColor = new Color(1f, 0.5f, 0f); 
    public Color lowColor = Color.red;
    public System.Action OnEmotionEmpty;


    public Image fillImage;

    void Start()
    {
        CurrentEmotion = maxEmotion;

        emotionSlider.minValue = 0;
        emotionSlider.maxValue = maxEmotion;
        emotionSlider.value = CurrentEmotion;
    }

    void AnimateEmotion(float targetValue, bool increase)
    {
        LeanTween.cancel(emotionSlider.gameObject);

        float duration = increase ? upTweenTime : downTweenTime;

        LeanTween.value(
            emotionSlider.gameObject,
            emotionSlider.value,
            targetValue,
            duration
        )
        .setEase(increase ? LeanTweenType.easeOutBack : LeanTweenType.easeOutCubic)
        .setOnUpdate((float val) =>
        {
            emotionSlider.value = val;
            UpdateColor(val);


            if (val <= 0.001f)
            {
                emotionSlider.value = 0; 
                OnEmotionEmpty?.Invoke();
            }
        });
    }

    void UpdateColor(float value)
    {
        float percent = value / maxEmotion;

        if (percent > 0.6f)
            fillImage.color = highColor;
        else if (percent > 0.3f)
            fillImage.color = midColor;
        else
            fillImage.color = lowColor;
    }
    public void ApplyResult(bool hit)
    {
        if (hit)
            CurrentEmotion += gainOnHit;
        else
            CurrentEmotion -= lossOnMiss;

        CurrentEmotion = Mathf.Clamp(CurrentEmotion, 0, maxEmotion);

        AnimateEmotion(CurrentEmotion, hit);
    }
    void Update()
    {
        float newEmotion = CurrentEmotion - drainPerSecond * Time.deltaTime;
        newEmotion = Mathf.Clamp(newEmotion, 0, maxEmotion);

        if (newEmotion != CurrentEmotion)
        {
            CurrentEmotion = newEmotion;
            AnimateEmotion(CurrentEmotion, false);
        }
    }


}
