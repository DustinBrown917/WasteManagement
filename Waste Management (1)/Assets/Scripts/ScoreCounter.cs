using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    private static ScoreCounter instance_ = null;
    public static ScoreCounter Instance { get { return instance_; } }

    [SerializeField] private Text label;

    public float Score { get { return score_; } }
    private float score_;

    private void Awake()
    {
        if(instance_ == null)
        {
            instance_ = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.StartGame += GameManager_StartGame;
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }

    private void GameManager_StartGame(object sender, System.EventArgs e)
    {
        ResetScore();
    }

    public void ResetScore()
    {
        SetScore(0);
    }

    public void AddScore(float amount)
    {
        score_ += amount;
        UpdateLabel();
    }

    private void SetScore(float score_)
    {
        this.score_ = score_;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        label.text = ((int)score_).ToString();
    }
}
