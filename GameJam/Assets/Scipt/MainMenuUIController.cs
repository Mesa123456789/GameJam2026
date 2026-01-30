using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI startButtonText;

    [Header("Scene")]
    public string gameSceneName = "1";

    void Start()
    {
        RefreshLanguage();
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void SelectThai()
    {
        LanguageManager.Instance.SetLanguage(GameLanguage.Thai);
        RefreshLanguage();
    }

    public void SelectEnglish()
    {
        LanguageManager.Instance.SetLanguage(GameLanguage.English);
        RefreshLanguage();
    }

    void RefreshLanguage()
    {
        if (LanguageManager.Instance.currentLanguage == GameLanguage.English)
        {
            titleText.text = "Emotion Control";
            startButtonText.text = "Start Game";
        }
        else
        {
            titleText.text = "ควบคุมอารมณ์";
            startButtonText.text = "เริ่มเกม";
        }
    }
}
