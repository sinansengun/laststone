using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public AudioScript BackgroundAudio;
    public MessageScript MessageScript;
    public GameObject MessagePanel;
    public GameObject SettingsPanel;
    public GameObject TutorialPanel;
    public GameObject BackgroundMusicPrefab;

    private void Start()
    {
        var backgroundMusic = GameObject.FindGameObjectWithTag("BackgroundMusic");
        if (backgroundMusic == null) {

            var position = Camera.main.gameObject.transform.position;
            Instantiate(BackgroundMusicPrefab, position, Quaternion.identity);
        }

        BackgroundAudio.PlayLoadSound2();
        SocialScript.Activate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (!IsAnyPanelActive()) {
                BackgroundAudio.PlayButtonSound();
                ShowQuitMessage();
            }
            else {
                BackgroundAudio.PlayButtonNegativeSound();
                SetPanelToPassive();
            }
        }
    }

    public void StartButton_Click()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitButton_Click()
    {
        ShowQuitMessage();
    }

    public void SocialButton_Click()
    {
        if (SocialScript.Authenticated) {
            MessageScript.Show("Are you sure want to sign out?", MessageBoxButton.YesNo, delegate(MessageBoxResult result) {
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
        var settingsScript = SettingsPanel.GetComponent<SettingsScript>();
        if (settingsScript != null) {
            settingsScript.Show();
        }
    }

    public void TutorialButton_OnClick()
    {
        SetPanelToPassive();

        var tutorialScript = TutorialPanel.GetComponent<TutorialScript>();
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
        if (SettingsPanel.activeInHierarchy ||
            TutorialPanel.activeInHierarchy) {
            return true;
        }
        return false;
    }

    public void ShowQuitMessage()
    {
        MessageScript.Show("Are you sure want to quit?", MessageBoxButton.YesNo, delegate(MessageBoxResult result) {
            if (result == MessageBoxResult.Yes) {
                Application.Quit();
            }
        });
    }

    private void SetPanelToPassive()
    {
        MessagePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        TutorialPanel.SetActive(false);
    }
}
