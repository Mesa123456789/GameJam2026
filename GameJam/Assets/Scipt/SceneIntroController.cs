using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class SceneIntroController : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Typing Text")]
    public TextMeshProUGUI typingText;
    [TextArea]
    public string introText = "Control your emotion.";
    public float typingSpeed = 0.05f;
    public float holdAfterTyping = 1f;

    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // 🔒 หยุดเวลาเกมก่อน
        Time.timeScale = 0f;

        // เตรียม UI
        fadeImage.gameObject.SetActive(true);
        typingText.gameObject.SetActive(true);
        typingText.text = "";
        StartCoroutine(TypeText(introText));
        // เริ่มเฟด (ไม่รอ)
        StartCoroutine(Fade(1f, 0f));

        // พิมพ์ข้อความไปพร้อมกับเฟด
        yield return StartCoroutine(TypeText(introText));


        // ค้างให้อ่าน
        yield return new WaitForSecondsRealtime(holdAfterTyping);

        // ซ่อนข้อความ
        typingText.gameObject.SetActive(false);

        // เปิดเวลาเกม
        Time.timeScale = 1f;
        fadeImage.gameObject.SetActive(false);
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
