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
    void Awake()
    {
        sceneResultConfig = FindObjectOfType<SceneResultConfig>();
    }

    public void ShowResult(int resultIndex)
    {
        popupRoot.SetActive(true);
        Cursor.visible = true;
        GameLanguage lang = LanguageManager.Instance.currentLanguage;

        ResultDataSO data =
            sceneResultConfig.GetResultByLanguage(lang);

        ResultEntry entry = data.results[resultIndex];

        titleText.text = entry.resultName;
        descriptionText.text = entry.description;

        foreach (var star in stars)
            star.transform.localScale = Vector3.zero;

        popupRoot.transform.localScale = Vector3.zero;

        LeanTween.scale(popupRoot, Vector3.one, popupScaleTime)
                 .setEaseOutBack();

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
                });
        }

        ShowNextButton();
    }
    void ShowNextButton()
    {
        if (nextButton == null) return;

        nextButton.transform.localScale = Vector3.zero;
        LeanTween.scale(nextButton, Vector3.one, 0.25f)
                 .setEaseOutBack();
    }
    public void GoToNextScene()
    {

        Time.timeScale = 1f; 
        SceneManager.LoadScene(nextSceneName);
    }

}
