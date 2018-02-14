using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour 
{
    private int visibleScore = 0;
    private int currentScore = 0;

    public float animationTime;
    public string defaultValue;

    public void ResetScore()
    {
        currentScore = 0;
        visibleScore = 0;
    }

    public void SetScore(int score)
    {
        currentScore = score;

        gameObject.ValueTo(new Hashtable{
            {"from", visibleScore},
            {"to", currentScore},
            {"onupdate", "UpdateScore"},
            {"time", animationTime}
        });
    }

    public void UpdateScore(int score)
    {
        visibleScore = score;

        var textComponent = gameObject.GetComponent<Text>();
        if (textComponent != null) {
            textComponent.text = score != 0 ? score.ToString() : defaultValue;
        }
    }
}
