using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using UnityEngine;

public class PlacarJogadores : MonoBehaviour
{
    [SerializeField] const string LeaderboardId = "Adventures";
    [SerializeField] TextMeshProUGUI topDez;
    [SerializeField] TextMeshProUGUI playerId;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] TextMeshProUGUI playerRank;
    string VersionId { get; set; }
    int Offset { get; set; }
    int Limit { get; set; }
    int RangeLimit { get; set; }
    List<string> FriendIds { get; set; }




    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        GetScores();
    }

    public async void AddScore()
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, 12345);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void GetScores()
    {
        topDez.text = "";
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        foreach(var score in scoresResponse.Results)
        {
            topDez.text += score.Rank + " - \t" + score.PlayerName + " - \t\t\t" + score.Score + "\n";
        }
        playerId.text = "Player ID: " + scoresResponse.Results.FindIndex(s => s.PlayerId == AuthenticationService.Instance.PlayerId).ToString();
        playerScore.text = "Player Score: " + scoresResponse.Results.Find(s => s.PlayerId == AuthenticationService.Instance.PlayerId)?.Score.ToString();
        playerRank.text = "Player Rank: " + scoresResponse.Results.Find(s => s.PlayerId == AuthenticationService.Instance.PlayerId)?.Rank.ToString();
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPaginatedScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPlayerScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void GetPlayerRange()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetPlayerRangeAsync(LeaderboardId, new GetPlayerRangeOptions { RangeLimit = RangeLimit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetScoresByPlayerIds()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresByPlayerIdsAsync(LeaderboardId, FriendIds);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
