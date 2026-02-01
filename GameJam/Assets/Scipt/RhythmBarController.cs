using UnityEngine;
using UnityEngine.UI;

public class RhythmBarController : MonoBehaviour
{
    public enum EmotionType
    {
        Smile,    // 🙂 +10
        Neutral,  // 😐 -10
        Angry     // 😠 -15
    }

    [Header("Slots")]
    public Image[] slotImages;          // ต้องมี 6 ช่อง
    public Sprite smileSprite;
    public Sprite neutralSprite;
    public Sprite angrySprite;

    [Header("Indicator")]
    public RectTransform indicator;
    public float indicatorMoveSpeed = 6f; // index ต่อวินาที

    [Header("Timing")]
    public float speedMultiplier = 2f;

    private EmotionType[] emotionSlots;
    private int currentIndex;
    private float indexTimer;

    public int CurrentIndex => currentIndex;
    public EmotionType CurrentEmotion => emotionSlots[currentIndex];
    private bool isRunning = true;

    void Start()
    {
        emotionSlots = new EmotionType[slotImages.Length];
        RandomizeSlots();
        MoveIndicatorInstant();
    }

    void Update()
    {
        if (!isRunning) return;

        UpdateIndicator();
    }

    // 🔁 ตัวชี้วิ่งวน index
    void UpdateIndicator()
    {
        indexTimer += Time.deltaTime * indicatorMoveSpeed;

        int newIndex = Mathf.FloorToInt(indexTimer) % slotImages.Length;

        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            MoveIndicatorInstant();
        }
    }

    void MoveIndicatorInstant()
    {
        Vector3 slotPos =
            slotImages[currentIndex].rectTransform.position;

        indicator.position =
            new Vector3(
                slotPos.x,
                slotPos.y + -60f,
                slotPos.z
            );
    }

    public void RandomizeSlots()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            EmotionType randomEmotion =
                (EmotionType)Random.Range(0, 3);

            emotionSlots[i] = randomEmotion;
            slotImages[i].sprite = GetSprite(randomEmotion);
        }
    }

    Sprite GetSprite(EmotionType type)
    {
        switch (type)
        {
            case EmotionType.Smile:
                return smileSprite;
            case EmotionType.Neutral:
                return neutralSprite;
            case EmotionType.Angry:
                return angrySprite;
        }
        return null;
    }

    // 📤 ให้ GameManager มาเรียก
    public EmotionType GetCurrentEmotion()
    {
        return emotionSlots[currentIndex];
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // 🎚 เรียกจาก GameManager เพื่อเร่งความเร็วตามเวลา
    public void UpdateSpeedByTime(float timeProgress01)
    {
        indicatorMoveSpeed =
            Mathf.Lerp(6f, 12f, timeProgress01 * speedMultiplier);
    }
    public void PlayHitFeedback()
    {
        // Indicator punch scale
        LeanTween.cancel(indicator.gameObject);
        indicator.localScale = Vector3.one;

        LeanTween.scale(
            indicator,
            Vector3.one * 1.5f,
            0.1f
        ).setEasePunch();

        // Slot pulse
        RectTransform slot =
            slotImages[currentIndex].rectTransform;

        LeanTween.cancel(slot.gameObject);
        slot.localScale = Vector3.one;

        LeanTween.scale(
            slot,
            Vector3.one * 1.5f,
            0.1f
        ).setEasePunch();
    }
    public void PlayMissFeedback()
    {
        // Indicator shake
        LeanTween.cancel(indicator.gameObject);
        indicator.localRotation = Quaternion.identity;

        LeanTween.rotateZ(
            indicator.gameObject,
            12f,
            0.08f
        ).setEaseShake()
         .setLoopPingPong(1);

        // Slot shake + shrink
        RectTransform slot =
            slotImages[currentIndex].rectTransform;

        LeanTween.cancel(slot.gameObject);
        slot.localScale = Vector3.one;

        LeanTween.scale(
            slot,
            Vector3.one * 0.9f,
            0.12f
        ).setEaseOutBack()
         .setLoopPingPong(1);
    }

    public void StopRhythm() { isRunning = false; }
}
