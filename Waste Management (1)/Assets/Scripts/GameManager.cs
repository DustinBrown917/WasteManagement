using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private static GameManager instance_ = null;
    public static GameManager Instance { get { return instance_; } }

    private static Camera mainCamera_ = null;
    public static Camera MainCamera { get
        {
            if(mainCamera_ == null) { mainCamera_ = Camera.main; }
            return mainCamera_;
        }
    }

    private GameStates gameState_;
    public GameStates GameState { get { return gameState_; } }

    public static bool isTouchBlocked;

    [SerializeField] private Timer timer;
    [SerializeField] private GameObject pushToStartPanel;
    [SerializeField] private HighscoreManager highScoreManager;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
            gameState_ = GameStates.MAIN_MENU;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timer.TimeOut += Timer_TimeOut;
    }

    private void Timer_TimeOut(object sender, System.EventArgs e)
    {       
        OnGameOver();
    }

    private void OnDestroy()
    {
        if(instance_ == this) {
            timer.TimeOut -= Timer_TimeOut;
            instance_ = null;
        }
    }

    public void StartPlaying()
    {
        pushToStartPanel.SetActive(false);
        OnStartGame();
    }

    public event EventHandler GameOver;

    private void OnGameOver()
    {
        Debug.Log("GameOver!");
        gameState_ = GameStates.MAIN_MENU;
        PlayGamesHandler.AddScoreToLeaderBoard(GPGSIds.leaderboard_high_score, (long)ScoreCounter.Instance.Score);
        pushToStartPanel.SetActive(true);
        EventHandler handler = GameOver;
        // Set the highScore if appropriate
        highScoreManager.UpdateHighScore(ScoreCounter.Instance.Score);

        if (handler != null) { handler(this, EventArgs.Empty); }
    }

    public event EventHandler StartGame;

    private void OnStartGame()
    {
        Debug.Log("GameStart!");
        gameState_ = GameStates.PLAYING;
        EventHandler handler = StartGame;
        if(handler != null) { handler(this, EventArgs.Empty); }
    }
}

public enum GameStates
{
    PLAYING,
    MAIN_MENU
}
