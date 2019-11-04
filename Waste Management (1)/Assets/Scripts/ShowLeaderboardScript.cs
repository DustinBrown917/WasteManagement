using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class ShowLeaderboardScript : MonoBehaviour
{
    public void ShowLeaderboard()
    {
        //PlayGamesHandler.ShowLeaderboard();
        Social.ShowLeaderboardUI();
        Debug.Log("Showing Leaderboard");
    }
}
