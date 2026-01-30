using UnityEngine;

[System.Serializable]
public class ResultEntry
{
    public string resultName;

    [TextArea(2, 4)]
    public string description;

    [Range(1, 3)]
    public int starCount;
}
