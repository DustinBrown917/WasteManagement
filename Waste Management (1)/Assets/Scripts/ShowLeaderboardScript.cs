using System.Collections;
using System.Collections.Generic;
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
