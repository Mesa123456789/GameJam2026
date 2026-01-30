using UnityEngine;

[CreateAssetMenu(
    fileName = "ResultData",
    menuName = "Game/EndGame Result Data"
)]
public class ResultDataSO : ScriptableObject
{
    public ResultEntry[] results;
}

