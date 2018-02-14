using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    private BannerView bannerViewAd;
    private InterstitialAd interstitialAd;

    private const int boardSize = 9;
    private int currentScore = 0;
    private int pawnCount = 0;
    private bool swipeEnabled = true;
    private Stopwatch movementTimer;
    private Transform[,] board;
    private Color[] boardColors;

    public AudioScript backgroundAudio;
    public GameObject currentScoreText;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject completePanel;
    public GameObject messagePanel;
    public GameObject tutorialPanel;
    public Transform pawnStoneShadow;
    public Transform pawnStone;
    public int maxMovementScore;
    public int maxMovementTime;

    private void Start()
    {
        if (!ShowTutorialIfNec(StartGame)) {
            StartGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (!IsAnyPanelActive()) {
                backgroundAudio.PlayButtonSound();
                PauseGame();
            }
            else {
                backgroundAudio.PlayButtonNegativeSound();
                SetPanelToPassive();
            }
        }
    }

    private void InitializeBoard()
    {
        board = new Transform[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++) {

                var positionX = x - 4;
                var positionY = y - 5;
                Instantiate(pawnStoneShadow, new Vector3(positionX, positionY, 0), Quaternion.identity);

                var pivot = (int)(boardSize / 2);
                if (pivot != x || pivot != y) {
                    InitializePawn(x, y, .2f);
                }
            }

        backgroundAudio.PlayLoadSound();
        movementTimer.Start();
    }

    private void InitializeObjects()
    {
        movementTimer = new Stopwatch();
        boardColors = new Color[10]{
            new Color(1f, .75f, .75f),
            new Color(.75f, .75f, 1f),
            new Color(.75f, .75f, .9f),
            new Color(.75f, 1f, .75f),
            new Color(1f, .75f, .75f),
            new Color(1f, 1f, .75f),
            new Color(.75f, 1f, 1f),
            new Color(1f, .75f, 1f),
            new Color(.75f, .75f, .75f),
            new Color(.9f, .9f, .9f)
        };
    }

    private void InitializePawn(int x, int y, float amount)
    {
        var positionX = x - 4;
        var positionY = y - 5;

        var pawnObject = Instantiate(pawnStone, new Vector3(positionX, positionY, -1), Quaternion.identity);
        var pawnTransform = (Transform)pawnObject;
        var pawn = pawnTransform.gameObject.GetComponent<PawnScript>();
        pawn.pointX = x;
        pawn.pointY = y;

        var pawnColor = boardColors[UnityEngine.Random.Range(0, boardColors.Length - 1)];
        var sprite = pawnTransform.gameObject.GetComponent<SpriteRenderer>();
        sprite.color = pawnColor;

        board[x, y] = pawnTransform;
        pawnCount++;

        ShakePawn(pawnTransform, .2f, amount);
    }

    private void StartGame()
    {
        InitializeObjects();
        InitializeBoard();

        PawnScript.OnPawnSwiped += PawnScript_OnPawnSwiped;
        StartCoroutine(AdStart());
    }

    private IEnumerator RestartGame()
    {
        AdInterstitialShow();
        StartCoroutine(AdInterstitialLoad());

        var pivot = (int)(boardSize / 2);
        var pivotPawn = board[pivot, pivot];
        if (pivotPawn != null) {
            DestroyPawn(pivotPawn);
        }

        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++) {

                if ((board[x, y] == null) &&
                    (pivot != x || pivot != y)) {

                    yield return new WaitForSeconds(.01f);
                    InitializePawn(x, y, .6f);
                }
            }

        var scoreScript = currentScoreText.GetComponent<ScoreScript>();
        if (scoreScript != null) {
            scoreScript.SetScore(0);
        }

        currentScore = 0;
        movementTimer.Reset();
        movementTimer.Start();
        backgroundAudio.PlayLoadSound();

        SetSwipeEnabled(true);
    }

    private void PauseGame()
    {
        var pauseScript = pausePanel.GetComponent<PauseScript>();
        if (pauseScript != null) {
            pauseScript.Show();
        }

        SetSwipeEnabled(false);
    }

    private void ResumeGame()
    {
        SetPanelToPassive();
        SetSwipeEnabled(true);
    }

    private bool CheckIsFinished()
    {
        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++) {

                var pawn = board[x, y];
                if (pawn == null) {
                    continue;
                }

                Transform nearPawn = null;
                Transform near2Pawn = null;

                var pawnScript = pawn.gameObject.GetComponent<PawnScript>();
                var pawnX = pawnScript.pointX;
                var pawnY = pawnScript.pointY;

                if (pawnX - 2 >= 0) {

                    nearPawn = board[pawnX - 1, pawnY];
                    near2Pawn = board[pawnX - 2, pawnY];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnX + 2 < boardSize) {

                    nearPawn = board[pawnX + 1, pawnY];
                    near2Pawn = board[pawnX + 2, pawnY];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnY + 2 < boardSize) {

                    nearPawn = board[pawnX, pawnY + 1];
                    near2Pawn = board[pawnX, pawnY + 2];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnY - 2 >= 0) {

                    nearPawn = board[pawnX, pawnY - 1];
                    near2Pawn = board[pawnX, pawnY - 2];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }
            }

        return true;
    }

    private void SwapIfNec(Transform selectedPawn, SwipeParameter swipeParameter)
    {
        var swipeDirection = swipeParameter.direction;
        if (!swipeEnabled || !(selectedPawn != null && swipeDirection != SwipeDirection.None)) {
            return;
        }

        var pawnScript = selectedPawn.gameObject.GetComponent<PawnScript>();
        var pawnX = pawnScript.pointX;
        var pawnY = pawnScript.pointY;

        Transform nearPawn = null;
        Transform near2Pawn = null;

        int near2PawnX = 0;
        int near2PawnY = 0;

        if (swipeDirection == SwipeDirection.Left) {

            if (pawnX - 2 < 0) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = board[pawnX - 1, pawnY];
            near2Pawn = board[near2PawnX = pawnX - 2, near2PawnY = pawnY];
        }
        else if (swipeDirection == SwipeDirection.Right) {

            if (pawnX + 2 >= boardSize) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = board[pawnX + 1, pawnY];
            near2Pawn = board[near2PawnX = pawnX + 2, near2PawnY = pawnY];
        }
        else if (swipeDirection == SwipeDirection.Up) {

            if (pawnY + 2 >= boardSize) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = board[pawnX, pawnY + 1];
            near2Pawn = board[near2PawnX = pawnX, near2PawnY = pawnY + 2];
        }
        else if (swipeDirection == SwipeDirection.Down) {

            if (pawnY - 2 < 0) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = board[pawnX, pawnY - 1];
            near2Pawn = board[near2PawnX = pawnX, near2PawnY = pawnY - 2];
        }

        if (nearPawn != null &&
            near2Pawn == null) {
            MoveAndDestroyPawn(selectedPawn, nearPawn, near2PawnX, near2PawnY);
        }
        else {
            PunchPawn(selectedPawn);
        }
    }

    private void MoveAndDestroyPawn(Transform selectedPawn, Transform nearPawn, int near2PawnX, int near2PawnY)
    {
        var selectedPawnScript = selectedPawn.gameObject.GetComponent<PawnScript>();
        var nearPawnScript = nearPawn.gameObject.GetComponent<PawnScript>();

        board[selectedPawnScript.pointX, selectedPawnScript.pointY] = null;
        board[nearPawnScript.pointX, nearPawnScript.pointY] = null;
        board[near2PawnX, near2PawnY] = selectedPawn;

        selectedPawnScript.pointX = near2PawnX;
        selectedPawnScript.pointY = near2PawnY;

        MovePawn(selectedPawn, near2PawnX, near2PawnY);
        DestroyPawn(nearPawn);

        CalculateScore();

        if (CheckIsFinished()) {

            var completeScript = completePanel.GetComponent<CompleteScript>();
            if (completeScript != null) {
                completeScript.Show(pawnCount, currentScore);
            }
        }
    }

    private void CalculateScore()
    {
        var movementTime = movementTimer.Elapsed.Seconds <= maxMovementTime ?
            movementTimer.Elapsed.Seconds :
            maxMovementTime - 1;

        var movementScore = (int)(((maxMovementTime - movementTime) / (double)maxMovementTime) * maxMovementScore);
        currentScore += movementScore;

        movementTimer.Reset();
        movementTimer.Start();

        var scoreScript = currentScoreText.GetComponent<ScoreScript>();
        if (scoreScript != null) {
            scoreScript.SetScore(currentScore);
        }
    }

    private void PunchPawn(Transform pawn)
    {
        pawn.gameObject.PunchScale(new Vector3(.5f, .5f), .5f, 0);
    }

    private void ShakePawn(Transform pawn)
    {
        pawn.gameObject.ShakeScale(new Vector3(.5f, .5f), .5f, 0);
    }

    private void ShakePawn(Transform pawn, float time)
    {
        pawn.gameObject.ShakeScale(new Vector3(.5f, .5f), time, 0);
    }

    private void ShakePawn(Transform pawn, float amount, float time)
    {
        pawn.gameObject.ShakeScale(new Vector3(amount, amount), time, 0);
    }

    private void ScalePawn(Transform pawn, float amount, float time)
    {
        pawn.gameObject.ScaleTo(new Vector3(amount, amount), time, 0);
    }

    private void MovePawn(Transform selectedPawn, int x, int y)
    {
        var positionX = x - 4;
        var positionY = y - 5;

        selectedPawn.gameObject.PunchScale(new Vector3(.5f, .5f), .5f, 0);
        selectedPawn.gameObject.MoveTo(new Vector3(positionX, positionY, -1), .5f, 0);
        backgroundAudio.PlaySwipeSound();
    }

    private void DestroyPawn(Transform pawnTransform)
    {
        Destroy(pawnTransform.gameObject);
        pawnCount--;
    }

    private void SetPanelToPassive()
    {
        var pauseScript = pausePanel.GetComponent<PauseScript>();
        if (pauseScript != null) {
            pauseScript.Close();
        }

        var settingsScript = settingsPanel.GetComponent<SettingsScript>();
        if (settingsScript != null) {
            settingsScript.Close();
        }

        var completeScript = completePanel.GetComponent<CompleteScript>();
        if (completeScript != null) {
            completeScript.Close();
        }

        var tutorialScript = tutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null) {
            tutorialScript.Close();
        }
    }

    private bool IsAnyPanelActive()
    {
        if (settingsPanel.activeInHierarchy ||
            completePanel.activeInHierarchy ||
            tutorialPanel.activeInHierarchy) {
            return true;
        }
        return false;
    }

    private void SetSwipeEnabled(bool enable)
    {
        swipeEnabled = enable;
    }

    private void PawnScript_OnPawnSwiped(Transform selectedPawn, SwipeParameter swipeParameter)
    {
        SwapIfNec(selectedPawn, swipeParameter);
    }

    public void HomeButton_Click()
    {
        Application.LoadLevel("Start");
    }

    public void ResumeButton_Click()
    {
        ResumeGame();
    }

    public void BackButton_OnClick()
    {
        PauseGame();
    }

    public void RestartButton_Click()
    {
        SetPanelToPassive();
        StartCoroutine(RestartGame());
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
        ShowTutorial();
    }

    public void ShowTutorial()
    {
        var tutorialScript = tutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null) {
            tutorialScript.Show();
        }
    }

    public bool ShowTutorialIfNec(Action onClosed)
    {
        var tutorialScript = tutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null &&
            tutorialScript.ShowInitially(onClosed)) {

            backgroundAudio.PlayLoadSound3();
            return true;
        }
        return false;
    }

    private IEnumerator AdStart()
    {
        yield return new WaitForSeconds(5);

        bannerViewAd = new BannerView(Consts.BannerIdentifier, AdSize.SmartBanner, AdPosition.Bottom);
        bannerViewAd.LoadAd(new AdRequest.Builder().Build());

        yield return new WaitForSeconds(10);

        interstitialAd = new InterstitialAd(Consts.InterstitialIdentifier);
        interstitialAd.LoadAd(new AdRequest.Builder().Build());
    }

    private IEnumerator AdInterstitialLoad()
    {
        yield return new WaitForSeconds(10);

        if (interstitialAd == null) {
            interstitialAd = new InterstitialAd(Consts.InterstitialIdentifier);
        }

        if (interstitialAd.IsLoaded() == false) {
            interstitialAd.LoadAd(new AdRequest.Builder().Build());
        }
    }

    private void AdInterstitialShow()
    {
        if (interstitialAd != null &&
            interstitialAd.IsLoaded()) {
            interstitialAd.Show();
        }
    }

    private void AdDestroy()
    {
        if (bannerViewAd != null) {
            bannerViewAd.Destroy();
            bannerViewAd = null;
        }

        if (interstitialAd != null) {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }

    public void OnDestroy()
    {
        AdDestroy();
        //Reklam yüklemesi yapılabilecek tüm durumlar iptal ediliyor.
        StopCoroutine(AdStart());
        StopCoroutine(AdInterstitialLoad());
    }
}
