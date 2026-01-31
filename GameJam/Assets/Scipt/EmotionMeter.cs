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

    [Header("Emotion State Sound")]
    public AudioSource emotionAudio;     // AudioSource ของ Emotion
    public AudioClip highEmotionClip;
    public AudioClip midEmotionClip;
    public AudioClip lowEmotionClip;

    public float emotionFadeTime = 0.4f;

    private EmotionState currentState;
    [Header("Emotion Settings")]
    public float maxEmotion = 100f;
    public float drainPerSecond = 5f;
    public float gainOnHit = 10f;
    public float lossOnMiss = 15f;
    private bool isFrozen = false;

    [Header("Handle Sprites")]
    public Image handleImage;

    public Sprite highHandleSprite;   // อารมณ์ดี
    public Sprite midHandleSprite;    // กลาง
    public Sprite lowHandleSprite;    // แย่

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
    public System.Action<EmotionState> OnEmotionStateChanged;



    public Image fillImage;

    void Start()
    {
        CurrentEmotion = maxEmotion;

        emotionSlider.minValue = 0;
        emotionSlider.maxValue = maxEmotion;
        emotionSlider.value = CurrentEmotion;

        currentState = GetEmotionState(CurrentEmotion);
        ApplyVisualByState(currentState);
    }


    void AnimateEmotion(float targetValue, bool increase)
    {
        if (isFrozen) return; 

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
    CheckEmotionStateChange(val);

    if (!isFrozen && val <= 0.001f)
    {
        emotionSlider.value = 0;
        OnEmotionEmpty?.Invoke();
    }
});

    }
    void UpdateColor(float value)
    {
        if (value < 40f)
            fillImage.color = lowColor;
        else if (value < 60f)
            fillImage.color = midColor;
        else
            fillImage.color = highColor;
    }


    void UpdateHandleSprite(float percent)
    {
        if (handleImage == null) return;

        if (percent > 0.6f && handleImage.sprite != highHandleSprite)
        {
            handleImage.sprite = highHandleSprite;
        }
        else if (percent > 0.3f && handleImage.sprite != midHandleSprite)
        {
            handleImage.sprite = midHandleSprite;
        }
        else if (handleImage.sprite != lowHandleSprite)
        {
            handleImage.sprite = lowHandleSprite;
        }
    }
    EmotionState GetEmotionState(float value)
    {
        if (value < 40f)
            return EmotionState.Low;
        else if (value < 60f)
            return EmotionState.Mid;
        else
            return EmotionState.High;
    }


    void ChangeEmotionState(EmotionState newState)
    {
        currentState = newState;
        PlayEmotionSound(newState, false);
    }

    void PlayEmotionSound(EmotionState state, bool instant)
    {
        if (emotionAudio == null) return;

        AudioClip targetClip = null;

        switch (state)
        {
            case EmotionState.High:
                targetClip = highEmotionClip;
                break;
            case EmotionState.Mid:
                targetClip = midEmotionClip;
                break;
            case EmotionState.Low:
                targetClip = lowEmotionClip;
                break;
        }

        if (targetClip == null) return;

        LeanTween.cancel(emotionAudio.gameObject);

        if (instant)
        {
            emotionAudio.clip = targetClip;
            emotionAudio.volume = 1f;
            emotionAudio.Play();
            return;
        }

        // 🔻 fade out
        LeanTween.value(emotionAudio.gameObject, emotionAudio.volume, 0f, emotionFadeTime * 0.5f)
            .setOnUpdate(v => emotionAudio.volume = v)
            .setOnComplete(() =>
            {
                emotionAudio.clip = targetClip;
                emotionAudio.Play();

                // 🔺 fade in
                LeanTween.value(emotionAudio.gameObject, 0f, 1f, emotionFadeTime * 0.5f)
                    .setOnUpdate(v => emotionAudio.volume = v);
            });
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
        if (isFrozen) return;
        float newEmotion = CurrentEmotion - drainPerSecond * Time.deltaTime;
        newEmotion = Mathf.Clamp(newEmotion, 0, maxEmotion);

        if (newEmotion != CurrentEmotion)
        {
            CurrentEmotion = newEmotion;
            AnimateEmotion(CurrentEmotion, false);
        }

    }

    public float GetCurrentEmotionValue()
    {
        return emotionSlider.value;
    }
    public void FreezeEmotion()
    {
        isFrozen = true;

        LeanTween.cancel(emotionSlider.gameObject);
    }
    void CheckEmotionStateChange(float value)
    {
        EmotionState newState = GetEmotionState(value);

        if (newState == currentState)
            return; // ⭐ ไม่เปลี่ยน ไม่กระพริบ

        currentState = newState;

        ApplyVisualByState(newState);

        OnEmotionStateChanged?.Invoke(newState);
    }
    void ApplyVisualByState(EmotionState state)
    {
        switch (state)
        {
            case EmotionState.High:
                handleImage.sprite = highHandleSprite;
                break;

            case EmotionState.Mid:
                handleImage.sprite = midHandleSprite;
                break;

            case EmotionState.Low:
                handleImage.sprite = lowHandleSprite;
                break;
        }
    }


}
