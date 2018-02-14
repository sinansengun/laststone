using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstantMessageScript : MonoBehaviour 
{
    public Transform messagePrefab;

    public void Show(string message, float amount)
    {
        StartCoroutine(Initialize(message, amount));      
    }

    private IEnumerator Initialize(string message, float amount)
    {
        var messageObject = Instantiate(messagePrefab);
        var messageTransform = (Transform)messageObject;
        messageTransform.SetParent(gameObject.transform, false);

        var messageText = messageTransform.GetComponent<Text>();
        if (messageText != null) {
            messageText.text = message;
        }

        yield return new WaitForSeconds(amount);

        Destroy(messageTransform.gameObject);
    }
}
