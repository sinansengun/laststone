using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);       
    }

    private void Awake()
    {
        var bgm = PlayerPrefs.GetInt("BGM", 1) == 1;
        if (bgm == false) {
            GetComponent<AudioSource>().Stop();
        }
    }

    public void SetPlay(bool play)
    {
        if (play) { GetComponent<AudioSource>().Play(); }
        else {
            GetComponent<AudioSource>().Stop();
        }
    }
}
