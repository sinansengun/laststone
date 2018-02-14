using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour
{
    public bool soundEnabled = true;
    public AudioClip buttonSound;
    public AudioClip buttonNegativeSound;
    public AudioClip swipeSound;
    public AudioClip loadSound;
    public AudioClip loadSound2;
    public AudioClip loadSound3;
    public AudioClip completeSound;
    public AudioClip loadNegativeSound;
    public AudioClip toogleSound;
    public AudioClip scoreSound;

    private AudioSource source;
    private float volLowRange = .75f;
    private float volHighRange = 1.0f;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        soundEnabled = PlayerPrefs.GetInt("Sound", 1) == 1; 
    }

    public void SetEnabled(bool enabled)
    {
        soundEnabled = enabled;
    }

    public void PlayButtonSound()   
    {
        if (soundEnabled && buttonSound != null) {
            source.PlayOneShot(buttonSound);
        }
    }

    public void PlayButtonNegativeSound()
    {
        if (soundEnabled && buttonNegativeSound != null) {
            source.PlayOneShot(buttonNegativeSound);
        }
    }

    public void PlaySwipeSound()
    {
        if (soundEnabled && swipeSound != null) {
            var volume = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(swipeSound, volume);
        }
    }

    public void PlayLoadSound()
    {
        if (soundEnabled && loadSound != null) {
            source.PlayOneShot(loadSound);
        }
    }

    public void PlayLoadSound2()
    {
        if (soundEnabled && loadSound2 != null) {
            source.PlayOneShot(loadSound2);
        }
    }

    public void PlayLoadSound3()
    {
        if (soundEnabled && loadSound3 != null) {
            source.PlayOneShot(loadSound3);
        }
    }

    public void PlayLoadNegativeSound()
    {
        if (soundEnabled && loadNegativeSound != null) {
            source.PlayOneShot(loadNegativeSound);
        }
    }

    public void PlayToogleSound()
    {
        if (soundEnabled && toogleSound != null) {
            source.PlayOneShot(toogleSound);
        }
    }

    public void PlayScoreSound()
    {
        if (soundEnabled && scoreSound != null) {
            source.PlayOneShot(scoreSound);
        }
    }

    public void PlayCompleteSound()
    {
        if (soundEnabled && completeSound != null) {
            source.PlayOneShot(completeSound);
        }
    }
}
