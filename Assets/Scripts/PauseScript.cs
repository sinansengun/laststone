using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour 
{
    public void Show()
    {
        Open();
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
}
