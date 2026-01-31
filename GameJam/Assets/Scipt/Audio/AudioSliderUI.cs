using UnityEngine;
using UnityEngine.UI;

public class AudioSliderUI : MonoBehaviour
{
    public Slider master;
    public Slider bgm;
    public Slider sfx;
    public Slider ui;

    void Start()
    {
        master.value = AudioManager.Instance.masterVolume;
        bgm.value = AudioManager.Instance.bgmVolume;
        sfx.value = AudioManager.Instance.sfxVolume;
        ui.value = AudioManager.Instance.uiVolume;

        master.onValueChanged.AddListener(AudioManager.Instance.SetMaster);
        bgm.onValueChanged.AddListener(AudioManager.Instance.SetBGM);
        sfx.onValueChanged.AddListener(AudioManager.Instance.SetSFX);
        ui.onValueChanged.AddListener(AudioManager.Instance.SetUI);
    }
}
