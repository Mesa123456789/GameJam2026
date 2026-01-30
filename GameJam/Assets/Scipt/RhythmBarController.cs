using UnityEngine;

public class RhythmBarController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform triangle;
    public RectTransform bar;
    public RectTransform redZone;

    [Header("Speed")]
    public float baseSpeed = 220f;
    public float speedMultiplier = 2.0f; 

    [Header("Red Zone Width")]
    public float baseZoneWidth = 160f;
    public float minZoneWidth = 30f;

    [Header("Difficulty Curve")]
    public AnimationCurve difficultyCurve;

    private float direction = 1f;
    private float barHalfWidth;

    [Header("Red Zone Spawn")]
    public float minDistanceFromLast = 120f;

    private float lastRedZoneX;

    [Header("Red Zone FX")]
    public float hitPunchScale = 0.2f;
    public float hitPunchTime = 0.15f;
    [Header("Red Zone Shake")]
    public float shakeAmount = 15f;
    public float shakeTime = 0.1f;

    void Start()
    {
        barHalfWidth = bar.rect.width / 2f;
        RelocateRedZone();
    }


    void MoveTriangle(float speed)
    {
        Vector2 pos = triangle.anchoredPosition;
        pos.x += direction * speed * Time.deltaTime;
        triangle.anchoredPosition = pos;

        if (Mathf.Abs(pos.x) >= barHalfWidth)
        {
            direction *= -1f;
        }
    }
    void PlayRedZoneHitFX()
    {
        LeanTween.cancel(redZone.gameObject);

        redZone.localScale = Vector3.one;

        LeanTween
            .scale(redZone, Vector3.one * (1f + hitPunchScale), hitPunchTime)
            .setEasePunch();
    }

    void ShakeAndRelocateRedZone()
    {
        LeanTween.cancel(redZone.gameObject);

        Vector3 originalPos = redZone.anchoredPosition;

        // 1️⃣ สั่นก่อน
        LeanTween
            .moveX(redZone, originalPos.x + shakeAmount, shakeTime / 2f)
            .setEase(LeanTweenType.easeShake)
            .setOnComplete(() =>
            {
                redZone.anchoredPosition = originalPos;

                // 2️⃣ ค่อยย้ายตำแหน่งจริง
                RelocateRedZone();
            });
    }

    public bool CheckHit()
    {
        float triX = triangle.anchoredPosition.x;
        float zoneX = redZone.anchoredPosition.x;
        float halfWidth = redZone.rect.width / 2f;

        bool hit = Mathf.Abs(triX - zoneX) <= halfWidth;

        if (hit)
        {
            PlayRedZoneHitFX(); // เด้ง (ถ้ามี)
        }

        // ⭐ สั่นก่อนย้ายตำแหน่ง (ทุกกรณี)
        ShakeAndRelocateRedZone();

        return hit;
    }

    public void UpdateRhythm(float timeProgress01)
    {
        float diff = difficultyCurve.Evaluate(timeProgress01);

        float speed = baseSpeed + (baseSpeed * speedMultiplier * diff);
        MoveTriangle(speed);
    }
    public void ApplyDifficultyStep(float timeProgress01)
    {
        float diff = difficultyCurve.Evaluate(timeProgress01);

        float zoneWidth = Mathf.Lerp(baseZoneWidth, minZoneWidth, diff);

        redZone.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            zoneWidth
        );
    }

    void RelocateRedZone()
    {
        float range = barHalfWidth * 0.8f;
        float newX;
        int safety = 0;

        do
        {
            newX = Random.Range(-range, range);
            safety++;
        }
        while (Mathf.Abs(newX - lastRedZoneX) < minDistanceFromLast && safety < 20);

        lastRedZoneX = newX;

        redZone.anchoredPosition =
            new Vector2(newX, redZone.anchoredPosition.y);
    }

}
