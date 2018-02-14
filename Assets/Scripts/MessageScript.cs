using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public enum MessageBoxButton
{
    OK = 0,
    OKCancel = 1,
    YesNoCancel = 3,
    YesNo = 4,
}

public enum MessageBoxResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 6,
    No = 7,
}

public class MessageScript : MonoBehaviour
{
    private event Action<MessageBoxResult> OnClosed;

    public Text messageBody;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject okButton;
    public GameObject cancelButton;

    public void Show(string message, MessageBoxButton button)
    {
        if (gameObject.activeInHierarchy) {
            Punch();
            return;
        }

        SetMessageBody(message);
        SetVisibility(button);
        Open();
    }

    public void Show(string message, MessageBoxButton button, Action<MessageBoxResult> onClosed)
    {
        if (gameObject.activeInHierarchy) {
            Punch();
            return;
        }

        OnClosed += onClosed;
        SetMessageBody(message);
        SetVisibility(button);
        Open();
    }

    private void SetMessageBody(string message)
    {
        messageBody.text = message;
    }

    private void SetVisibility(MessageBoxButton button)
    {
        switch (button)
        {
            case MessageBoxButton.OK:
                cancelButton.SetActive(false);
                yesButton.SetActive(false);
                noButton.SetActive(false);
                break;
            case MessageBoxButton.OKCancel:
                yesButton.SetActive(false);
                noButton.SetActive(false);
                break;
            case MessageBoxButton.YesNo:
                okButton.SetActive(false);
                cancelButton.SetActive(false);
                break;
            case MessageBoxButton.YesNoCancel:
                okButton.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void ButtonOk_Click()
    {
        if (OnClosed != null) {
            OnClosed(MessageBoxResult.OK);
        }
        Close();
    }

    public void ButtonYes_Click()
    {
        if (OnClosed != null) {
            OnClosed(MessageBoxResult.Yes);
        }
        Close();
    }

    public void ButtonNo_Click()
    {
        if (OnClosed != null) {
            OnClosed(MessageBoxResult.No);
        }
        Close();
    }

    public void ButtonCancel_Click()
    {
        if (OnClosed != null) {
            OnClosed(MessageBoxResult.Cancel);
        }
        Close();
    }

    private void Open()
    {
        gameObject.SetActive(true);
        gameObject.PunchScale(new Vector3(1, 1, 0), 0.5f, 0);
    }

    private void Close()
    {
        gameObject.SetActive(false);
        OnClosed = null;
    }

    private void Punch()
    {
        gameObject.PunchScale(new Vector3(1, 1, 0), 0.5f, 0);
    }
}
