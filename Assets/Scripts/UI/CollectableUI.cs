using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableUI : MonoBehaviour
{
    public int ChocolateBar = 0;
    public int ChocoBite = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();
        /*
        Controller controller = GetComponent<Controller>();
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();
        playerStats.OnChocolate += ChocolateCollect;
        if (playerStats != null)
        {
            playerStats.AddOnChocolateCallBack(ChocolateCollect);
            playerStats.AddOnChocoCallBack(ChocoCollect);
        }
        */
    }
     
    // Update is called once per frame
    void Update()
    {
        ChocolateCollect();
    }
    public void ChocolateCollect()
    {
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();

        ChocolateBar = playerStats.Collectables;
        playerStats.Collectables = ChocolateBar;
        ChocoBite = playerStats.chocoBites;
        playerStats.chocoBites = ChocoBite;
        print("chocolate");
    }

    public void ChocoCollect()
    {
        print("choco");
    } 
}
