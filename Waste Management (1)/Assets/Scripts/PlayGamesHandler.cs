using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayGamesHandler : MonoBehaviour
{
    void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }


    private void SignIn()
    {
        Social.localUser.Authenticate(success => { Debug.Log("Successful sign in!"); });
    }

    #region Leaderboards

    public static void AddScoreToLeaderBoard(string leaderboardID, long score)
    {
        Social.ReportScore(score, leaderboardID, callback => { Debug.Log("Score logged: " + score.ToString()); });
    }

    public static void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    #endregion
}
