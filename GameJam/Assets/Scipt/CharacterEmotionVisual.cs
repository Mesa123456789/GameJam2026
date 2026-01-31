using UnityEngine;
using UnityEngine.UI;
using static EmotionMeter;

public class CharacterEmotionVisual : MonoBehaviour
{
    [Header("UI Image")]
    public Image spriteRenderer;

    [Header("Sprites By Emotion")]
    public Sprite highSprite;
    public Sprite midSprite;
    public Sprite lowSprite;

    [Header("Bounce Animation")]
    public float bounceHeight = 30f;   // ⭐ UI ใช้ค่าเป็น pixel
    public float bounceTime = 0.2f;

    private EmotionState currentState;
    private RectTransform rectTransform;
    private Vector2 basePos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        basePos = rectTransform.anchoredPosition;
    }

    // ================= EMOTION =================

    public void SetEmotionState(EmotionState newState)
    {
        if (newState == currentState) return;

        currentState = newState;
        UpdateSprite(newState);
        Bounce();
    }

    void UpdateSprite(EmotionState state)
    {
        switch (state)
        {
            case EmotionState.High:
                spriteRenderer.sprite = highSprite;
                break;
            case EmotionState.Mid:
                spriteRenderer.sprite = midSprite;
                break;
            case EmotionState.Low:
                spriteRenderer.sprite = lowSprite;
                break;
        }
    }

    // ================= BOUNCE =================

    public void Bounce()
    {
        LeanTween.cancel(gameObject);

        rectTransform.anchoredPosition = basePos;

        LeanTween.moveY(
            rectTransform,
            basePos.y + bounceHeight,
            bounceTime * 0.5f
        )
        .setEaseOutQuad()
        .setOnComplete(() =>
        {
            LeanTween.moveY(
                rectTransform,
                basePos.y,
                bounceTime * 0.5f
            ).setEaseInQuad();
        });
    }
}
