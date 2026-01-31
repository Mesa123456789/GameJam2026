using UnityEngine;
using static EmotionMeter;

public class EmotionAudioController : MonoBehaviour
{
    [Header("Reference")]
    public EmotionMeter emotionMeter;
    public AudioSource audioSource;

    [Header("Emotion Clips")]
    public AudioClip lowClip;   // 😠 โกรธ
    public AudioClip midClip;   // 😐 กลาง
    public AudioClip highClip;  // 🙂 ดี

    [Header("Fade Settings")]
    public float fadeTime = 0.4f;

    EmotionState currentState;

    void OnEnable()
    {
        if (emotionMeter == null) return;

        emotionMeter.OnEmotionStateChanged += OnEmotionChanged;

        // ⭐ sync ตอนเริ่ม
        OnEmotionChanged(emotionMeter.CurrentState);
    }

    void OnDisable()
    {
        if (emotionMeter != null)
            emotionMeter.OnEmotionStateChanged -= OnEmotionChanged;
    }

    void OnEmotionChanged(EmotionState state)
    {
        if (state == currentState) return;

        currentState = state;

        AudioClip targetClip = GetClipByState(state);
        if (targetClip == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeTo(targetClip));
    }

    AudioClip GetClipByState(EmotionState state)
    {
        switch (state)
        {
            case EmotionState.Low:
                return lowClip;
            case EmotionState.Mid:
                return midClip;
            case EmotionState.High:
                return highClip;
        }
        return null;
    }

    System.Collections.IEnumerator FadeTo(AudioClip newClip)
    {
        // fade out
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // fade in
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        audioSource.volume = 1f;
    }
    public void StopEmotionAudio()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAndStop());
    }

    System.Collections.IEnumerator FadeOutAndStop()
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset เผื่อใช้ใหม่
    }

}
