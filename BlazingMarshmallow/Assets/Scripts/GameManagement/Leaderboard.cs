using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.PlayerSettings;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSave save;

    public LeaderboardSlot[] slots;

    public void Awake()
    {
        Player[] playersList = new Player[5];
        for(int i = 0; i < playersList.Length; i++)
        {
            playersList[i] = new Player();
        }
        save = new LeaderboardSave(playersList);
    }

    public void CreateAndAddPlayer(string name)
    {
        Player newPlayer = new Player();
        newPlayer.name = name;

        LevelTransitions levels = LevelTransitions.instance;
        if(levels != null ) {
            newPlayer.time = levels.playTime;
        }

        AddPlayerToLeaderBoard(newPlayer);
        UpdateLeaderboard();

        print("Tried update leaderboard");
    }

    public void AddPlayerToLeaderBoard(Player player)
    {
        float time = player.time;
        int pos = 0;
        for(int i = 0; i < save.players.Length; i++)
        {
            if (save.players[i].time > time) {
                pos = i;
            }
        }

        if(pos > 0)
        {
            save.players[pos] = player;
        }
    }

    public void UpdateLeaderboard()
    {
        for (int i = 0; i < save.players.Length; i++)
        {
            if(i < slots.Length)
            {
                slots[i].name.text = save.players[i].name;
                slots[i].time.text = save.players[i].time.ToString("#.00");
            }
        }

        Serializer.SaveLeaderboard(save, SceneManager.GetActiveScene().name);
    }
}
