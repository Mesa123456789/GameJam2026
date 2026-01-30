using UnityEngine;

public enum GameLanguage
{
    Thai,
    English
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    [Header("Current Language")]
    public GameLanguage currentLanguage = GameLanguage.English;

    [Header("Result Data")]
    public ResultDataSO thaiResult;
    public ResultDataSO englishResult;

    const string LANGUAGE_KEY = "GAME_LANGUAGE";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(GameLanguage language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt(LANGUAGE_KEY, (int)language);
    }

    void LoadLanguage()
    {
        if (PlayerPrefs.HasKey(LANGUAGE_KEY))
        {
            currentLanguage =
                (GameLanguage)PlayerPrefs.GetInt(LANGUAGE_KEY);
        }
    }

    public ResultDataSO GetCurrentResultData()
    {
        return currentLanguage == GameLanguage.English
            ? englishResult
            : thaiResult;
    }
}
