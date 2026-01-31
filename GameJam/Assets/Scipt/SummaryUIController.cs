using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummaryUIController : MonoBehaviour
{
    [System.Serializable]
    public class SummarySlot
    {
        public string sceneName;
        public SceneResultConfig sceneConfig;

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI infoText;
        public Image[] stars;
    }

    public SummarySlot[] slots;

    void Start()
    {
        GameLanguage lang =
            LanguageManager.Instance.currentLanguage;

        foreach (var slot in slots)
        {
            bool hasPlayed =
                ProgressManager.Instance
                    .TryGetSceneResult(slot.sceneName, out int resultIndex);

            if (!hasPlayed)
            {
                // ❌ ไม่เคยเล่นจริง ๆ
                ClearSlot(slot);
                continue;
            }

            // ✅ เคยเล่นแล้ว แม้ resultIndex = 0 ก็ถือว่าเล่นแล้ว
            ShowPlayedSlot(slot, resultIndex);


            ResultDataSO data =
                slot.sceneConfig.GetResultByLanguage(lang);

            ResultEntry entry =
                data.results[resultIndex];

            slot.nameText.text = entry.resultName;
            slot.infoText.text = entry.description;

            UpdateStars(slot.stars, entry.starCount);
        }
    }

    void UpdateStars(Image[] stars, int count)
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = i < count;
    }
    void ShowPlayedSlot(SummarySlot slot, int resultIndex)
    {
        GameLanguage lang =
            LanguageManager.Instance.currentLanguage;

        ResultDataSO data =
            slot.sceneConfig.GetResultByLanguage(lang);

        ResultEntry entry =
            data.results[resultIndex];

        slot.nameText.text = entry.resultName;
        slot.infoText.text = entry.description;

        UpdateStars(slot.stars, entry.starCount);
    }

    void ClearSlot(SummarySlot slot)
    {
        slot.nameText.text = "???";
        slot.infoText.text = "Not played yet";
        UpdateStars(slot.stars, 0);
    }
}
