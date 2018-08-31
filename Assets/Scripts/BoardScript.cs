using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardScript : MonoBehaviour
{
    private BannerView _bannerViewAd;
    private InterstitialAd _interstitialAd;

    private const int BoardSize = 9;

    private int _currentScore;
    private int _pawnCount;
    private bool _swipeEnabled = true;
    private Stopwatch _movementTimer;
    private Transform[,] _board;
    private Color[] _boardColors;

    public AudioScript BackgroundAudio;
    public GameObject CurrentScoreText;
    public GameObject PausePanel;
    public GameObject SettingsPanel;
    public GameObject CompletePanel;
    public GameObject MessagePanel;
    public GameObject TutorialPanel;
    //public GameObject GamePanel;
    public Transform PawnStoneShadow;
    public Transform PawnStone;
    public int MaxMovementScore;
    public int MaxMovementTime;

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
                BackgroundAudio.PlayButtonSound();
                PauseGame();
            }
            else {
                BackgroundAudio.PlayButtonNegativeSound();
                SetPanelToPassive();
            }
        }
    }

    private void InitializeBoard()
    {
        _board = new Transform[BoardSize, BoardSize];

        for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++) {

                var positionX = x - 4;
                var positionY = y - 5;
                var shadowObject = Instantiate(PawnStoneShadow);
                shadowObject.SetPositionAndRotation(new Vector3(positionX, positionY, -1), Quaternion.identity);

                const int pivot = BoardSize / 2;
                if (pivot != x || pivot != y) {
                    InitializePawn(x, y, .2f);
                }
            }

        BackgroundAudio.PlayLoadSound();
        _movementTimer.Start();
    }

    private void InitializeObjects()
    {
        _movementTimer = new Stopwatch();
        _boardColors = new Color[6]{
            new Color(.5f, 1f, 1f, .9f),
            new Color(.5f, .95f, 1f, .9f),
            new Color(.5f, .9f, 1f, .9f),
            new Color(.5f, .85f, 1f, .9f),
            new Color(.5f, .8f, 1f, .9f),
            new Color(.5f, .75f, 1f, .9f),
        };
    }

    private void InitializePawn(int x, int y, float amount)
    {
        var positionX = x - 4;
        var positionY = y - 5;

        var pawnObject = Instantiate(PawnStone);
        pawnObject.SetPositionAndRotation(new Vector3(positionX, positionY, -1), Quaternion.identity);

        var pawn = pawnObject.gameObject.GetComponent<PawnScript>();
        pawn.pointX = x;
        pawn.pointY = y;

        var pawnColor = _boardColors[UnityEngine.Random.Range(0, _boardColors.Length - 1)];
        var sprite = pawnObject.gameObject.GetComponent<SpriteRenderer>();
        sprite.color = pawnColor;

        _board[x, y] = pawnObject;
        _pawnCount++;

        ShakePawn(pawnObject, .2f, amount);
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

        var pivot = (int)(BoardSize / 2);
        var pivotPawn = _board[pivot, pivot];
        if (pivotPawn != null) {
            DestroyPawn(pivotPawn);
        }

        for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++) {

                if ((_board[x, y] == null) &&
                    (pivot != x || pivot != y)) {

                    yield return new WaitForSeconds(.01f);
                    InitializePawn(x, y, .6f);
                }
            }

        var scoreScript = CurrentScoreText.GetComponent<ScoreScript>();
        if (scoreScript != null) {
            scoreScript.SetScore(0);
        }

        _currentScore = 0;
        _movementTimer.Reset();
        _movementTimer.Start();

        BackgroundAudio.PlayLoadSound();
        SetSwipeEnabled(true);
    }

    private void PauseGame()
    {
        var pauseScript = PausePanel.GetComponent<PauseScript>();
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
        for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++) {

                var pawn = _board[x, y];
                if (pawn == null) {
                    continue;
                }

                Transform nearPawn = null;
                Transform near2Pawn = null;

                var pawnScript = pawn.gameObject.GetComponent<PawnScript>();
                var pawnX = pawnScript.pointX;
                var pawnY = pawnScript.pointY;

                if (pawnX - 2 >= 0) {

                    nearPawn = _board[pawnX - 1, pawnY];
                    near2Pawn = _board[pawnX - 2, pawnY];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnX + 2 < BoardSize) {

                    nearPawn = _board[pawnX + 1, pawnY];
                    near2Pawn = _board[pawnX + 2, pawnY];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnY + 2 < BoardSize) {

                    nearPawn = _board[pawnX, pawnY + 1];
                    near2Pawn = _board[pawnX, pawnY + 2];

                    if (nearPawn != null && near2Pawn == null) {
                        return false;
                    }
                }

                if (pawnY - 2 >= 0) {

                    nearPawn = _board[pawnX, pawnY - 1];
                    near2Pawn = _board[pawnX, pawnY - 2];

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
        if (!_swipeEnabled || !(selectedPawn != null && swipeDirection != SwipeDirection.None)) {
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

            nearPawn = _board[pawnX - 1, pawnY];
            near2Pawn = _board[near2PawnX = pawnX - 2, near2PawnY = pawnY];
        }
        else if (swipeDirection == SwipeDirection.Right) {

            if (pawnX + 2 >= BoardSize) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = _board[pawnX + 1, pawnY];
            near2Pawn = _board[near2PawnX = pawnX + 2, near2PawnY = pawnY];
        }
        else if (swipeDirection == SwipeDirection.Up) {

            if (pawnY + 2 >= BoardSize) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = _board[pawnX, pawnY + 1];
            near2Pawn = _board[near2PawnX = pawnX, near2PawnY = pawnY + 2];
        }
        else if (swipeDirection == SwipeDirection.Down) {

            if (pawnY - 2 < 0) {
                PunchPawn(selectedPawn);
                return;
            }

            nearPawn = _board[pawnX, pawnY - 1];
            near2Pawn = _board[near2PawnX = pawnX, near2PawnY = pawnY - 2];
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

        _board[selectedPawnScript.pointX, selectedPawnScript.pointY] = null;
        _board[nearPawnScript.pointX, nearPawnScript.pointY] = null;
        _board[near2PawnX, near2PawnY] = selectedPawn;

        selectedPawnScript.pointX = near2PawnX;
        selectedPawnScript.pointY = near2PawnY;

        MovePawn(selectedPawn, near2PawnX, near2PawnY);
        DestroyPawn(nearPawn);

        CalculateScore();

        if (CheckIsFinished()) {

            var completeScript = CompletePanel.GetComponent<CompleteScript>();
            if (completeScript != null) {
                completeScript.Show(_pawnCount, _currentScore);
            }
        }
    }

    private void CalculateScore()
    {
        var movementTime = _movementTimer.Elapsed.Seconds <= MaxMovementTime ?
            _movementTimer.Elapsed.Seconds :
            MaxMovementTime - 1;

        var movementScore = (int)(((MaxMovementTime - movementTime) / (double)MaxMovementTime) * MaxMovementScore);
        _currentScore += movementScore;

        _movementTimer.Reset();
        _movementTimer.Start();

        var scoreScript = CurrentScoreText.GetComponent<ScoreScript>();
        if (scoreScript != null) {
            scoreScript.SetScore(_currentScore);
        }
    }

    private static void PunchPawn(Transform pawn)
    {
        pawn.gameObject.PunchScale(new Vector3(.5f, .5f), .5f, 0);
    }

    private static void ShakePawn(Transform pawn, float amount, float time)
    {
        pawn.gameObject.ShakeScale(new Vector3(amount, amount), time, 0);
    }

    private void MovePawn(Transform selectedPawn, int x, int y)
    {
        var positionX = x - 4;
        var positionY = y - 5;

        selectedPawn.gameObject.PunchScale(new Vector3(.5f, .5f), .5f, 0);
        selectedPawn.gameObject.MoveTo(new Vector3(positionX, positionY, -1), .5f, 0);
        BackgroundAudio.PlaySwipeSound();
    }

    private void DestroyPawn(Transform pawnTransform)
    {
        Destroy(pawnTransform.gameObject);
        _pawnCount--;
    }

    private void SetPanelToPassive()
    {
        var pauseScript = PausePanel.GetComponent<PauseScript>();
        if (pauseScript != null) {
            pauseScript.Close();
        }

        var settingsScript = SettingsPanel.GetComponent<SettingsScript>();
        if (settingsScript != null) {
            settingsScript.Close();
        }

        var completeScript = CompletePanel.GetComponent<CompleteScript>();
        if (completeScript != null) {
            completeScript.Close();
        }

        var tutorialScript = TutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null) {
            tutorialScript.Close();
        }
    }

    private bool IsAnyPanelActive()
    {
        if (SettingsPanel.activeInHierarchy ||
            CompletePanel.activeInHierarchy ||
            TutorialPanel.activeInHierarchy) {
            return true;
        }
        return false;
    }

    private void SetSwipeEnabled(bool enable)
    {
        _swipeEnabled = enable;
    }

    private void PawnScript_OnPawnSwiped(Transform selectedPawn, SwipeParameter swipeParameter)
    {
        SwapIfNec(selectedPawn, swipeParameter);
    }

    public void HomeButton_Click()
    {
        SceneManager.LoadScene("Start");
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
        var settingsScript = SettingsPanel.GetComponent<SettingsScript>();
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
        var tutorialScript = TutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null) {
            tutorialScript.Show();
        }
    }

    public bool ShowTutorialIfNec(Action onClosed)
    {
        var tutorialScript = TutorialPanel.GetComponent<TutorialScript>();
        if (tutorialScript != null &&
            tutorialScript.ShowInitially(onClosed)) {

            BackgroundAudio.PlayLoadSound3();
            return true;
        }
        return false;
    }

    private IEnumerator AdStart()
    {
        yield return new WaitForSeconds(5);

        _bannerViewAd = new BannerView(Consts.BannerIdentifier, AdSize.SmartBanner, AdPosition.Bottom);
        _bannerViewAd.LoadAd(new AdRequest.Builder().Build());

        yield return new WaitForSeconds(10);

        _interstitialAd = new InterstitialAd(Consts.InterstitialIdentifier);
        _interstitialAd.LoadAd(new AdRequest.Builder().Build());
    }

    private IEnumerator AdInterstitialLoad()
    {
        yield return new WaitForSeconds(10);

        if (_interstitialAd == null) {
            _interstitialAd = new InterstitialAd(Consts.InterstitialIdentifier);
        }

        if (_interstitialAd.IsLoaded() == false) {
            _interstitialAd.LoadAd(new AdRequest.Builder().Build());
        }
    }

    private void AdInterstitialShow()
    {
        if (_interstitialAd != null &&
            _interstitialAd.IsLoaded()) {
            _interstitialAd.Show();
        }
    }

    private void AdDestroy()
    {
        if (_bannerViewAd != null) {
            _bannerViewAd.Destroy();
            _bannerViewAd = null;
        }

        if (_interstitialAd != null) {
            _interstitialAd.Destroy();
            _interstitialAd = null;
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
