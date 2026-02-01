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
            // 👉 อ่านค่าที่บันทึกไว้ (ยังเป็น resultIndex แบบเดิม)
            bool hasPlayed =
                ProgressManager.Instance
                    .TryGetSceneResult(slot.sceneName, out int resultIndex);

            if (!hasPlayed)
            {
                ClearSlot(slot);
                continue;
            }

            ResultDataSO data =
                slot.sceneConfig.GetResultByLanguage(lang);

            ResultEntry entry = ResolveResultEntry(data, resultIndex);

            if (entry == null)
            {
                // กันพังขั้นสุดท้าย
                Debug.LogWarning(
                    $"[SUMMARY] Cannot resolve result for scene {slot.sceneName}, index {resultIndex}"
                );
                ClearSlot(slot);
                continue;
            }

            slot.nameText.text = entry.resultName;
            slot.infoText.text = entry.description;

            UpdateStars(slot.stars, entry.starCount);
        }
    }

    // ================= HOTFIX CORE =================

    /// <summary>
    /// แก้บัค resultIndex ไม่ตรงกับ starCount
    /// </summary>
    ResultEntry ResolveResultEntry(ResultDataSO data, int resultIndex)
    {
        if (data == null || data.results == null || data.results.Length == 0)
            return null;

        // 1️⃣ ลองใช้ index ตรง ๆ ก่อน (3 ดาวมักจะถูก)
        if (resultIndex >= 0 && resultIndex < data.results.Length)
        {
            ResultEntry direct = data.results[resultIndex];

            // ถ้า starCount ตรงกับ index+1 ถือว่าใช้ได้
            if (direct.starCount == resultIndex + 1)
                return direct;
        }

        // 2️⃣ fallback: หา entry จาก starCount (index+1)
        int expectedStar = resultIndex + 1;

        foreach (var r in data.results)
        {
            if (r.starCount == expectedStar)
                return r;
        }

        // 3️⃣ กันพังสุดท้าย: คืน entry แรก
        return data.results[0];
    }

    // ================= UI HELPERS =================

    void UpdateStars(Image[] stars, int starCount)
    {
        if (stars == null) return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].enabled = i < starCount;
        }
    }

    void ClearSlot(SummarySlot slot)
    {
        slot.nameText.text = "???";
        slot.infoText.text = "Not played yet";
        UpdateStars(slot.stars, 0);
    }
}
