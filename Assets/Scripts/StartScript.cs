using UnityEngine;
using System.Collections;
using GooglePlayGames;

public class StartScript : MonoBehaviour
{
    public AudioScript backgroundAudio;
    public MessageScript messageScript;
    public GameObject messagePanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public GameObject backgroundMusicPrefab;

    private void Start()
    {
        var backgroundMusic = GameObject.FindGameObjectWithTag("BackgroundMusic");
        if (backgroundMusic == null) {

            var position = Camera.main.gameObject.transform.position;
            Instantiate(backgroundMusicPrefab, position, Quaternion.identity);
        }

        backgroundAudio.PlayLoadSound2();
        SocialScript.Activate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (!IsAnyPanelActive()) {
                backgroundAudio.PlayButtonSound();
                ShowQuitMessage();
            }
            else {
                backgroundAudio.PlayButtonNegativeSound();
                SetPanelToPassive();
            }
        }
    }

    public void StartButton_Click()
    {
        Application.LoadLevel("Main");
    }

    public void ExitButton_Click()
    {
        ShowQuitMessage();
    }

    public void SocialButton_Click()
    {
        if (SocialScript.Authenticated) {
            messageScript.Show("Are you sure want to sign out?", MessageBoxButton.YesNo, delegate(MessageBoxResult result) {
                if (result == MessageBoxResult.Yes) {
                    SocialScript.SignOut();                    
                }
            });
        }
        else {

            SocialScript.Authenticate();                         
        }
    }

    public void SettingsButton_OnClick()
    {
        var settingsScript = settingsPanel.GetComponent<SettingsScript>();
        if (settingsScript != null) {
            settingsScript.Show();
        }
    }

    public void TutorialButton_OnClick()
    {
        SetPanelToPassive();

        var tutorialScript = tutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null) {
            tutorialScript.Show();
        }
    }

    public void ScoreButton_OnClick()
    {
        SocialScript.AuthenticateAndShow();
    }

    public void ResumeButton_Click()
    {
        SetPanelToPassive();
    }

    private bool IsAnyPanelActive()
    {
        if (settingsPanel.activeInHierarchy ||
            tutorialPanel.activeInHierarchy) {
            return true;
        }
        return false;
    }

    public void ShowQuitMessage()
    {
        messageScript.Show("Are you sure want to quit?", MessageBoxButton.YesNo, delegate(MessageBoxResult result) {
            if (result == MessageBoxResult.Yes) {
                Application.Quit();
            }
        });
    }

    private void SetPanelToPassive()
    {
        messagePanel.SetActive(false);
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }
}
