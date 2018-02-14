using UnityEngine;
using System.Collections;
using System;

public class TutorialScript : MonoBehaviour 
{
    private event Action OnClosed;

    public void Show()
    {
        Open();
        PlayerPrefs.SetInt("Tutorial", 1);
    }

    public bool ShowInitially(Action onClosed)
    {
        var tutorial = PlayerPrefs.GetInt("Tutorial", 0) == 0;
        if (tutorial) {
            OnClosed += onClosed;
            Show();

            return true;
        }
        return false;
    }

    private void Open()
    {
        gameObject.SetActive(true);
        gameObject.PunchScale(new Vector3(1, 1, 0), 0.5f, 0);
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (OnClosed != null) {
            OnClosed();
            OnClosed = null;
        }
    }
}
