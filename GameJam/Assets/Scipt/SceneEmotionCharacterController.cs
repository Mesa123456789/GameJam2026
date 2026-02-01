using static EmotionMeter;
using UnityEngine;

public class SceneEmotionCharacterController : MonoBehaviour
{
    public EmotionMeter emotionMeter;

    public CharacterEmotionVisual leftCharacter;
    public CharacterEmotionVisual rightCharacter;

    void OnEnable()
    {
        if (emotionMeter == null) return;

        emotionMeter.OnEmotionStateChanged += OnEmotionChanged;

        OnEmotionChanged(emotionMeter.CurrentState);
    }

    void OnDisable()
    {
        if (emotionMeter != null)
            emotionMeter.OnEmotionStateChanged -= OnEmotionChanged;
    }


    void OnEmotionChanged(EmotionState state)
    {
        leftCharacter.SetEmotionState(state);
        rightCharacter.SetEmotionState(state);
    }
}
