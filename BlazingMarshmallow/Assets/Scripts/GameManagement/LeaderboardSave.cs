using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardSave
{
    public Player[] players;

    public LeaderboardSave(Player[] inPlayers) {
        players = inPlayers;
    }
}

[System.Serializable]
public class Player
{
    public string name;
    public float time;
    public int chocoBites;
    public int chocolates;

    public Player() {
        time = float.PositiveInfinity;
        name = "---";
    }
}
