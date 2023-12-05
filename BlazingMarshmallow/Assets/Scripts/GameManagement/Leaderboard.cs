using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.PlayerSettings;
using UnityEngine.SceneManagement;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSave save;

    public LeaderboardSlot[] slots;

    public TMP_InputField inputField;

    public void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        LevelTransitions levels = LevelTransitions.instance;
        if(levels != null )
        {
            levels.leaderboards.TryGetValue(sceneName, out save);
        }

        if (save == null)
        {
            print("Default leaderboard");
            Player[] playersList = new Player[5];
            for (int i = 0; i < playersList.Length; i++)
            {
                playersList[i] = new Player();
            }
            save = new LeaderboardSave(playersList);
        }

        UpdateLeaderboard();
        CheckIfPlayerOnLeaderBoard();
    }

    public void CheckIfPlayerOnLeaderBoard()
    {
        LevelTransitions levels = LevelTransitions.instance;
        if (levels != null)
        {
            if (save.players.First<Player>().time > levels.playTime)
            {
                inputField.interactable = true;
                inputField.text = "Enter name...";
                return;
            }
        }

        inputField.interactable = false;
        inputField.text = "Try for the fastest time!";
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


        inputField.interactable = false;

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

                if(i - 1 < save.players.Length && i - 1 >= 0) {
                    save.players[i - 1] = save.players[i];
                }
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
            if (i < slots.Length)
            {
                slots[i].name.text = save.players[i].name;

                if (float.IsInfinity(save.players[i].time))
                {
                    slots[i].time.text = "-";
                }
                else
                {
                    slots[i].time.text = save.players[i].time.ToString("#.00");
                }
            }
        }

        Serializer.SaveLeaderboard(save, SceneManager.GetActiveScene().name);

        LevelTransitions levels = LevelTransitions.instance;
        if (levels != null)
        {
            levels.LoadLeaderboards();
        }
    }
}
