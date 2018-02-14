using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompleteScript : MonoBehaviour 
{
    //0: One Star
    //1: Double Star
    //2: Triple Star
    public Sprite[] starSprites;
    public GameObject starObject;
    public GameObject instantMessagePanel;
    public Text currentScoreText;
    public Text bestScoreText;
    public int starScore;

    public void Show(int pawnCount, int currentScore)
    {
        gameObject.SetActive(true);
        gameObject.PunchScale(new Vector3(1, 1, 0), 0.5f, 0);

        Initialize(pawnCount, currentScore);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Initialize(int pawnCount, int currentScore)
    {
        var bestScore = PlayerPrefs.GetInt("BestScore", 0);
        SetCurrentScore(currentScore);
        SetBestScore(bestScore);
        PlayLoadSound();

        StartCoroutine(InitializeScores(pawnCount, currentScore, bestScore));
    }

    public IEnumerator InitializeScores(int pawnCount,  int currentScore, int bestScore)
    {
        var starImage = starObject.GetComponent<Image>();
        starImage.gameObject.SetActive(false);

        if (pawnCount == 1) {

            starImage.gameObject.SetActive(true);
            starImage.sprite = starSprites[2];
            currentScore += (starScore * 3);

            yield return new WaitForSeconds(1f);
            ShowInstantMessage("+{0}".F(starScore * 3));
            yield return new WaitForSeconds(1f);
            SetCurrentScore(currentScore);
        }
        else if (pawnCount == 2) {

            starImage.gameObject.SetActive(true);
            starImage.sprite = starSprites[1];
            currentScore += (starScore * 2);

            yield return new WaitForSeconds(1f);
            ShowInstantMessage("+{0}".F(starScore * 2));
            yield return new WaitForSeconds(1f);
            SetCurrentScore(currentScore);
        }
        else if (pawnCount == 3) {

            starImage.gameObject.SetActive(true);
            starImage.sprite = starSprites[0];
            currentScore += (starScore * 1);

            yield return new WaitForSeconds(1f);
            ShowInstantMessage("+{0}".F((starScore * 1)));
            yield return new WaitForSeconds(1f);
            SetCurrentScore(currentScore);
        }

        if (currentScore > bestScore) {

            yield return new WaitForSeconds(1f);
            ShowInstantMessage("New Best Score\n{0}".F(currentScore));
            yield return new WaitForSeconds(1f);

            SetBestScore(currentScore);
            PlayerPrefs.SetInt("BestScore", currentScore);
            SocialScript.ReportScore(currentScore);
        }
    }

    private void SetBestScore(int bestScore)
    {
        var bestScoreScript = bestScoreText.GetComponent<ScoreScript>();
        if (bestScoreScript != null) {
            bestScoreScript.SetScore(bestScore);
        }
    }

    private void SetCurrentScore(int currentScore)
    {
        var currentScoreScript = currentScoreText.GetComponent<ScoreScript>();
        if (currentScoreScript != null) {
            currentScoreScript.SetScore(currentScore);
        }
    }

    private void ShowInstantMessage(string message)
    {
        var instantMessageScript = instantMessagePanel.GetComponent<InstantMessageScript>();
        if (instantMessageScript != null) {
            instantMessageScript.Show(message, 4f);
        }
    }

    private void PlayLoadSound()
    {
        var backgroundAudio = GameObject.FindGameObjectWithTag("BackgroundAudio");
        var backgroundAudioScript = backgroundAudio.GetComponent<AudioScript>();

        backgroundAudioScript.PlayCompleteSound();
    }
}
