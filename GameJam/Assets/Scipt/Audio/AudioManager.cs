using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    const string MASTER_KEY = "VOL_MASTER";
    const string BGM_KEY = "VOL_BGM";
    const string SFX_KEY = "VOL_SFX";
    const string UI_KEY = "VOL_UI";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
            ApplyAll(); // ⭐ สำคัญ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ================= SET FROM SLIDER =================

    public void SetMaster(float v)
    {
        masterVolume = v;
        Save();
        ApplyAll();
    }

    public void SetBGM(float v)
    {
        bgmVolume = v;
        Save();
        ApplyAll();
    }

    public void SetSFX(float v)
    {
        sfxVolume = v;
        Save();
        ApplyAll();
    }

    public void SetUI(float v)
    {
        uiVolume = v;
        Save();
        ApplyAll();
    }

    // ================= APPLY =================

    public void ApplyAll()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>(true);

        foreach (var src in sources)
        {
            ApplyVolume(src);
        }
    }

    public void ApplyVolume(AudioSource src)
    {
        if (src == null) return;

        float categoryVolume = 1f;

        // 🎵 เพลง
        if (src.CompareTag("BGM"))
        {
            categoryVolume = bgmVolume;
        }
        // 💥 เสียงเกม
        else if (src.CompareTag("SFX"))
        {
            categoryVolume = sfxVolume;
        }
        // 🖱 เสียง UI
        else if (src.CompareTag("UI"))
        {
            categoryVolume = uiVolume;
        }

        // ⭐ master คูณทุกอย่าง
        src.volume = masterVolume * categoryVolume;
    }

    // ================= SAVE / LOAD =================

    void Save()
    {
        PlayerPrefs.SetFloat(MASTER_KEY, masterVolume);
        PlayerPrefs.SetFloat(BGM_KEY, bgmVolume);
        PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);
        PlayerPrefs.SetFloat(UI_KEY, uiVolume);
    }

    void LoadVolume()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        uiVolume = PlayerPrefs.GetFloat(UI_KEY, 1f);
    }
}
