using UnityEngine;

public class SceneResultConfig : MonoBehaviour
{
    [Header("Result Data For This Scene")]
    public ResultDataSO thaiResult;
    public ResultDataSO englishResult;

    public ResultDataSO GetResultByLanguage(GameLanguage language)
    {
        return language == GameLanguage.English
            ? englishResult
            : thaiResult;
    }
}
