using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour 
{
    public Toggle bgmToggle;
    public Toggle soundToggle;

    public void Show()
    {
        Initialize();
        Open();
    }

    private void Initialize()
    {
        var bgm = PlayerPrefs.GetInt("BGM", 1) == 1;
        var sound = PlayerPrefs.GetInt("Sound", 1) == 1;

        bgmToggle.isOn = bgm;
        soundToggle.isOn = sound;
    }

    private void Open()
    {
        gameObject.SetActive(true);
        gameObject.PunchScale(new Vector3(1, 1, 0), 0.5f, 0);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SoundToogle_ValueChanged(bool toggleChecked)
    {
        var backgroundAudio = GameObject.FindGameObjectWithTag("BackgroundAudio");
        var backgroundAudioScript = backgroundAudio.GetComponent<AudioScript>();

        backgroundAudioScript.SetEnabled(toggleChecked);

        PlayerPrefs.SetInt("Sound", toggleChecked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void BgmToogle_ValueChanged(bool toggleChecked)
    {
        var backgroundMusic = GameObject.FindGameObjectWithTag("BackgroundMusic");
        var backgroundMusicScript = backgroundMusic.GetComponent<MusicScript>();

        backgroundMusicScript.SetPlay(toggleChecked);

        PlayerPrefs.SetInt("BGM", toggleChecked ? 1 : 0);
        PlayerPrefs.Save();
    }
}
