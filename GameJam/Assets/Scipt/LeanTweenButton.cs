using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeanTweenButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    public enum IdleAnimationType
    {
        None,
        Float,     
        Pulse    
    }
    [Header("Idle Animation")]
    public IdleAnimationType idleType = IdleAnimationType.None;

    public float idleFloatDistance = 5f;
    public float idleFloatTime = 1.2f;

    public float idlePulseScale = 1.05f;
    public float idlePulseTime = 1.2f;

    private int idleTweenId = -1;
    private Vector3 idleBasePos;
    private Vector3 idleBaseScale;

    [Header("Scale Settings")]
    public float hoverScale = 1.05f;
    public float pressScale = 0.95f;

    [Header("Tween Time")]
    public float hoverTime = 0.12f;
    public float pressTime = 0.08f;
    public float releaseTime = 0.18f;

    [Header("Optional Scene Load")]
    public string sceneToLoad = "";

    [Header("Click Sound")]
    public AudioSource clickAudio;

    private Vector3 defaultScale;
    private bool isPressed;
    private Button button;

    void Awake()
    {
        defaultScale = transform.localScale;
        idleBaseScale = transform.localScale;
        idleBasePos = transform.localPosition;
        button = GetComponent<Button>();

        if (clickAudio == null)
            clickAudio = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        transform.localScale = defaultScale;
        transform.localPosition = idleBasePos;
        isPressed = false;

        StartIdle();
    }
    void StartIdle()
    {
        if (idleType == IdleAnimationType.None)
            return;

        StopIdle();

        if (idleType == IdleAnimationType.Float)
        {
            idleTweenId = LeanTween.moveLocalY(
                gameObject,
                idleBasePos.y + idleFloatDistance,
                idleFloatTime * 0.5f
            )
            .setEaseInOutSine()
            .setLoopPingPong()
            .id;
        }
        else if (idleType == IdleAnimationType.Pulse)
        {
            idleTweenId = LeanTween.scale(
                gameObject,
                idleBaseScale * idlePulseScale,
                idlePulseTime * 0.5f
            )
            .setEaseInOutSine()
            .setLoopPingPong()
            .id;
        }
    }

    void StopIdle()
    {
        if (idleTweenId != -1)
        {
            LeanTween.cancel(idleTweenId);
            idleTweenId = -1;
        }

        transform.localScale = idleBaseScale;
        transform.localPosition = idleBasePos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopIdle();

        if (isPressed) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, defaultScale * hoverScale, hoverTime)
                 .setEaseOutQuad();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPressed) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, defaultScale, hoverTime)
                 .setEaseOutQuad();

        StartIdle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopIdle();
        isPressed = true;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, defaultScale * pressScale, pressTime)
                 .setEaseInQuad();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, defaultScale * hoverScale, releaseTime)
                 .setEaseOutBack();

        StartIdle();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
     
        if (clickAudio != null)
            clickAudio.Play();

        if (button != null && button.onClick.GetPersistentEventCount() > 0)
            return;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
