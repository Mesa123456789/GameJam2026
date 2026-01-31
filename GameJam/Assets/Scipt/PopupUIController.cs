using UnityEngine;
using UnityEngine.UI;

public class PopupUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject popupRoot;
    public CanvasGroup fadeBG;
    public RectTransform popupBox;

    [Header("Buttons")]
    public RectTransform confirmButton;
    public RectTransform cancelButton;

    [Header("Animation")]
    public float fadeTime = 0.25f;
    public float popupScaleTime = 0.3f;
    public float buttonDelay = 0.15f;

    void Awake()
    {
        popupRoot.SetActive(false);
    }

    // ================= OPEN =================
    public void ShowPopup()
    {
        popupRoot.SetActive(true);

        // Reset
        fadeBG.alpha = 0;
        popupBox.localScale = Vector3.zero;

        if (confirmButton) confirmButton.localScale = Vector3.zero;
        if (cancelButton) cancelButton.localScale = Vector3.zero;

        // Fade BG
        LeanTween.alphaCanvas(fadeBG, 1f, fadeTime);

        // Popup scale
        LeanTween.scale(popupBox, Vector3.one, popupScaleTime)
                 .setEaseOutBack()
                 .setOnComplete(ShowButtons);
    }

    void ShowButtons()
    {
        if (confirmButton)
        {
            LeanTween.scale(confirmButton, Vector3.one, 0.25f)
                     .setEaseOutBack();
        }

        if (cancelButton)
        {
            LeanTween.delayedCall(buttonDelay, () =>
            {
                LeanTween.scale(cancelButton, Vector3.one, 0.25f)
                         .setEaseOutBack();
            });
        }
    }

    // ================= CLOSE =================
    public void HidePopup()
    {
        // Fade BG
        LeanTween.alphaCanvas(fadeBG, 0f, fadeTime);

        // Popup scale down
        LeanTween.scale(popupBox, Vector3.zero, 0.2f)
                 .setEaseInBack()
                 .setOnComplete(() =>
                 {
                     popupRoot.SetActive(false);
                 });
    }
    public void OnHover(RectTransform btn)
    {
        LeanTween.scale(btn, Vector3.one * 1.05f, 0.12f);
    }

    public void OnExit(RectTransform btn)
    {
        LeanTween.scale(btn, Vector3.one, 0.12f);
    }

}
