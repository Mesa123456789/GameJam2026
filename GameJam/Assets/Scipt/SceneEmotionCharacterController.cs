using UnityEngine;
using static EmotionMeter;

public class SceneEmotionCharacterController : MonoBehaviour
{
    public EmotionMeter emotionMeter;

    [Header("Characters")]
    public CharacterEmotionVisual leftCharacter;
    public CharacterEmotionVisual rightCharacter;

    public void BounceBoth()
    {
        if (leftCharacter != null)
            leftCharacter.Bounce();

        if (rightCharacter != null)
            rightCharacter.Bounce();
    }

}
