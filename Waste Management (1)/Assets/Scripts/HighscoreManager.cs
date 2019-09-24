using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{

    public static int score;

    public static int highscore;

    Text text;

    void Start()
    {
        text = GetComponent<Text>();

        score = 0;

        highscore = PlayerPrefs.GetInt("highscore", highscore);

        UpdateHighScore(highscore);
    }

    public void UpdateHighScore(float currentScore)
    {
        if (currentScore >= highscore)
        {
            highscore = (int)currentScore;
            text.text = "" + highscore;

            PlayerPrefs.SetInt("highscore", highscore);
        }
    }

    public static void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
    }
}