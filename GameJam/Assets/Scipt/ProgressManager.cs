using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;


    private Dictionary<string, int> sceneResults =
        new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSceneResult(string sceneName, int resultIndex)
    {
        if (sceneResults.ContainsKey(sceneName))
        {
 
            sceneResults[sceneName] =
                Mathf.Max(sceneResults[sceneName], resultIndex);
        }
        else
        {
            sceneResults.Add(sceneName, resultIndex);
        }
    }

    public bool TryGetSceneResult(string sceneName, out int resultIndex)
    {
        return sceneResults.TryGetValue(sceneName, out resultIndex);
    }

    public Dictionary<string, int> GetAllResults()
    {
        return sceneResults;
    }
}
