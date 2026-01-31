using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIController : MonoBehaviour
{
    [Header("Tabs")]
    public RectTransform btnLanguage;
    public RectTransform btnAudio;
    public RectTransform tabHighlightBG;

    [Header("Pages")]
    public GameObject pageLanguage;
    public GameObject pageAudio;

    [Header("Animation")]
    public float slideTime = 0.25f;

    void Start()
    {

        OpenLanguage();
    }



    public void OpenLanguage()
    {


        pageLanguage.SetActive(true); 
        pageAudio.SetActive(false);

        
    }

    public void OpenAudio()
    {
   
        pageLanguage.SetActive(false);
        pageAudio.SetActive(true);

    }

  

}
