using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class ShowLeaderboardScript : MonoBehaviour
{
    public void ShowLeaderboard()
    {
#if UNITY_ANDROID
        Social.ShowLeaderboardUI();
#endif
        Debug.Log("Showing Leaderboard");
    }
}
