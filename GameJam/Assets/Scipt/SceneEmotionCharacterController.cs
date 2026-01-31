using static EmotionMeter;
using UnityEngine;

public class SceneEmotionCharacterController : MonoBehaviour
{
    public EmotionMeter emotionMeter;

    public CharacterEmotionVisual leftCharacter;
    public CharacterEmotionVisual rightCharacter;

    void Start()
    {
        emotionMeter.OnEmotionStateChanged += OnEmotionChanged;
    }

    void OnEmotionChanged(EmotionState state)
    {
        // ⭐ เปลี่ยน sprite เท่านั้น
        leftCharacter.SetEmotionState(state);
        rightCharacter.SetEmotionState(state);
    }

    // ⭐ เด้งจาก input
    //public void BounceBoth()
    //{
    //    leftCharacter.Bounce();
    //    rightCharacter.Bounce();
    //}
}
