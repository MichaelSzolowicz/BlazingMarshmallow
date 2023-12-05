using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;

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
            if(!LevelTransitions.leaderboards.TryGetValue(sceneName, out save)) {
                Player[] players = new Player[slots.Length];
                for (int i = 0; i < slots.Length; i++)
                {
                    players[i] = new Player();
                }
                save = new LeaderboardSave(players);

                Serializer.SaveLeaderboard(save, SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            return;
        }
     
        UpdateLeaderboard();
        CheckIfPlayerOnLeaderBoard();
    }

    public void CheckIfPlayerOnLeaderBoard()
    {
        if(save == null)
        {
            return;
        }

        LevelTransitions levels = LevelTransitions.instance;
        if (levels != null)
        {
            Player p = save.players.FirstOrDefault<Player>();
            if (p != null && p.time > levels.playTime)
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
        if(save == null)
        {
            return;
        }

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
        if (save == null)
        {
            return;
        }

        float time = player.time;
        int pos = 0;
        for(int i = 0; i < save.players.Length; i++)
        {
            if (save.players[i].time > time) {
                pos = i;

                if(i - 1 >= 0) {
                    save.players[i - 1] = save.players[i];
                }
            }
        }

        if(pos >= 0)
        {
            save.players[pos] = player;
        }
    }

    public void UpdateLeaderboard()
    {
        if(save == null)
        {
            return;
        }

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
    }
}
