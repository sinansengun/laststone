using GooglePlayGames;
using UnityEngine;

public class SocialScript
{
    public static bool Authenticated { get { return Social.localUser.authenticated; } }

    static SocialScript()
    {
        PlayGamesPlatform.Activate();

        var social = PlayerPrefs.GetInt("Social") == 1;
        if (social) {
            Authenticate();
        }
    }

    public static void Activate() { }

    public static void Authenticate()
    {
        Social.localUser.Authenticate(success => {

            if (!success) return;

            ReportBestScore();
        });
    }

    public static void AuthenticateAndShow()
    {
        Social.localUser.Authenticate(success => {

            if (!success) return;

            ShowLeaderboard();
            ReportBestScore();
        });
    }

    public static void ShowLeaderboard()
    {
        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(Consts.LeaderBoardIdentifier);
    }

    public static void ReportScore(int score)
    {
        if (!Authenticated) return;

        Social.ReportScore(score, Consts.LeaderBoardIdentifier, success => { });
    }

    public static void ReportBestScore()
    {
        var bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScore != 0) {
            ReportScore(bestScore);
            PlayerPrefs.SetInt("Social", 1);
        }
    }

    public static void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
        PlayerPrefs.SetInt("Social", 0);
    }
}

