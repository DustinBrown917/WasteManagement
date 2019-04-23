using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text timerLabel;
    [SerializeField] private Image timerImage;
    [SerializeField] private Gradient imageGradient;

    [SerializeField] private float maxTime;
    private float timeRemaining;
    private int timeRemainingInt;
    private bool isRunning = false;
    private AudioSource audioSource;
    [SerializeField] private AudioClip tick;
    [SerializeField] private AudioClip tock;
    private bool tickTock;
    [SerializeField] private AudioClip cuckoo;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        GameManager.Instance.StartGame += GameManager_StartGame;
        GameManager.Instance.GameOver += GameManager_GameOver;
    }

    private void GameManager_GameOver(object sender, EventArgs e)
    {
        audioSource.clip = cuckoo;
        audioSource.Play();
    }

    private void GameManager_StartGame(object sender, EventArgs e)
    {
        ResetTimer();
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning) {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                StopTimer();
                OnTimeOut();
            }
            if (ShouldUpdateTimerLabel()) {
                tickTock = !tickTock;
                if (tickTock) { audioSource.clip = tick; }
                else { audioSource.clip = tock; }
                audioSource.Play();
                UpdateTimerLabel();
            }
            UpdateTimerImage();
        }
    }

    private void UpdateTimerLabel()
    {
        timeRemainingInt = (int)timeRemaining;
        timerLabel.text = timeRemainingInt.ToString();
    }

    private void UpdateTimerImage()
    {
        float t = timeRemaining / maxTime;
        timerImage.fillAmount = t;
        timerImage.color = imageGradient.Evaluate(t);
    }

    private bool ShouldUpdateTimerLabel()
    {
        return (timeRemainingInt == (int) timeRemaining)? false : true;
    }

    public void ResetTimer()
    {
        timeRemaining = maxTime;
        UpdateTimerImage();
        UpdateTimerLabel();
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public event EventHandler TimeOut;

    private void OnTimeOut()
    {
        EventHandler handler = TimeOut;

        if(handler != null) { handler(this, EventArgs.Empty); }
    }
}
