using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLeaderboardScript : MonoBehaviour
{
    public void ShowLeaderboard()
    {
        PlayGamesHandler.ShowLeaderboard();
        Debug.Log("Showing Leaderboard");
    }
}
