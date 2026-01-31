using UnityEngine;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI startButtonText;

    [Header("Language Dropdown")]
    public TMP_Dropdown languageDropdown;
    public GameObject option;
    void Start()
    {
        SetupDropdown();
        RefreshLanguage();
        option.SetActive(false);
    }

    void SetupDropdown()
    {
        // sync dropdown กับภาษาปัจจุบัน
        languageDropdown.value =
            LanguageManager.Instance.currentLanguage == GameLanguage.English
            ? 1
            : 0;

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void OnLanguageChanged(int index)
    {
        if (index == 1)
            LanguageManager.Instance.SetLanguage(GameLanguage.English);
        else
            LanguageManager.Instance.SetLanguage(GameLanguage.Thai);

        RefreshLanguage();
    }
    public void OpenOption()
    {
        option.SetActive(true);
    }
    public void back()
    {
        option.SetActive(false);
    }
    void RefreshLanguage()
    {
        if (LanguageManager.Instance.currentLanguage == GameLanguage.English)
        {
            titleText.text = "Emotional Damage";
            startButtonText.text = "Start";
        }
        else
        {
            titleText.text = "เก็บอาการหน่อยจ้า";
            startButtonText.text = "ปะ";
        }
    }
}
