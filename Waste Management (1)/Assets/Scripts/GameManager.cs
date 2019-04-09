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

    [SerializeField] private Timer timer;
    [SerializeField] private GameObject pushToStartPanel;

    public int score;
    public Text scoreText;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
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

    public void UpdateScore()
    {
        scoreText.text = "" + score;
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
        pushToStartPanel.SetActive(true);
        EventHandler handler = GameOver;
        if(handler != null) { handler(this, EventArgs.Empty); }
    }

    public event EventHandler StartGame;

    private void OnStartGame()
    {
        Debug.Log("GameStart!");
        EventHandler handler = StartGame;
        if(handler != null) { handler(this, EventArgs.Empty); }
    }
}
