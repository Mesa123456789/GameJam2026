using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Typing Text")]
    public TextMeshProUGUI typingText;
    [TextArea]
    public string introText = "Sometimes, controlling emotion is harder than it seems...";
    public float typingSpeed = 0.05f;
    public float holdAfterTyping = 1f; // เวลาค้างหลังพิมพ์จบ

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 🔒 หยุดเวลาเกมก่อน
        Time.timeScale = 0f;

        // เตรียม UI
        fadeImage.gameObject.SetActive(true);
        typingText.gameObject.SetActive(true);
        typingText.text = "";

        // Fade in (ดำ → ใส)
        yield return StartCoroutine(Fade(1f, 0f));

        // Typing text
        yield return StartCoroutine(TypeText(introText));

        // ค้างให้อ่าน
        yield return new WaitForSecondsRealtime(holdAfterTyping);

        // ปิด text
        typingText.gameObject.SetActive(false);

        // เปิดเวลาเกม
        Time.timeScale = 1f;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    IEnumerator TypeText(string text)
    {
        typingText.text = "";

        foreach (char c in text)
        {
            typingText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}
