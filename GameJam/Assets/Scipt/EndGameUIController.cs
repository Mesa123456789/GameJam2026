using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameUIController : MonoBehaviour
{
    [Header("Data")]
    private ResultDataSO resultData;
    private SceneResultConfig sceneResultConfig;
    [Header("UI")]
    public GameObject popupRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image[] stars;

    [Header("Animation")]
    public float popupScaleTime = 0.3f;
    public float starDelay = 0.25f;
    [Header("Next Scene")]
    public string nextSceneName;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject retryButton;
    [Header("End Result Sound")]
    public AudioSource resultAudioSource;
    public AudioClip badResultClip;
    public AudioClip normalResultClip;
    public AudioClip goodResultClip;

    [Header("UI Popup Sound")]
    public AudioSource uiAudioSource;
    public AudioClip popupOpenClip;
    public AudioClip starPopClip;


    void Awake()
    {
        sceneResultConfig = FindObjectOfType<SceneResultConfig>();
    }

    public void ShowResult(int resultIndex)
    {
        popupRoot.SetActive(true);

        Cursor.visible = true;

        // 🔔 popup sound
        PlayPopupSound();

        // 🏆 result sound
        PlayEndResultSound(resultIndex);

        GameLanguage lang = LanguageManager.Instance.currentLanguage;
        ResultDataSO data = sceneResultConfig.GetResultByLanguage(lang);
        ResultEntry entry = data.results[resultIndex];

        titleText.text = entry.resultName;
        descriptionText.text = entry.description;

        foreach (var star in stars)
            star.transform.localScale = Vector3.zero;

        popupRoot.transform.localScale = Vector3.zero;

        LeanTween.scale(popupRoot, Vector3.one, popupScaleTime)
                 .setEaseOutBack();

        // ⭐ star + sound
        for (int i = 0; i < entry.starCount; i++)
        {
            int index = i;
            LeanTween.delayedCall(gameObject,
                popupScaleTime + starDelay * i,
                () =>
                {
                    LeanTween.scale(
                        stars[index].gameObject,
                        Vector3.one,
                        0.3f
                    ).setEaseOutBack();

                    PlayStarSound();
                });
        }

        ShowButton(nextButton);
        ShowButton(retryButton);
    }

    void ShowButton(GameObject button)
    {
        if (button == null) return;

        button.transform.localScale = Vector3.zero;
        LeanTween.scale(button, Vector3.one, 0.25f)
                 .setEaseOutBack();
    }
    void PlayEndResultSound(int resultIndex)
    {
        if (resultAudioSource == null) return;

        AudioClip clip = null;

        switch (resultIndex)
        {
            case 0:
                clip = badResultClip;
                break;
            case 1:
                clip = normalResultClip;
                break;
            case 2:
                clip = goodResultClip;
                break;
        }

        if (clip != null)
        {
            resultAudioSource.PlayOneShot(clip);
        }
    }
    void PlayPopupSound()
    {
        if (uiAudioSource != null && popupOpenClip != null)
        {
            uiAudioSource.PlayOneShot(popupOpenClip);
        }
    }
    void PlayStarSound()
    {
        if (uiAudioSource != null && starPopClip != null)
        {
            uiAudioSource.PlayOneShot(starPopClip);
        }
    }


}
