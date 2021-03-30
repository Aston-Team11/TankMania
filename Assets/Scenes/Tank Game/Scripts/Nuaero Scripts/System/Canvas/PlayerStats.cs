using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private InGameMenus playerStats;
    [SerializeField] private int playerNum;
   private Text stats;  //the number of kills or the nubmer of player lives depending onwhich gamemode is active

    // Start is called before the first frame update
    void Start()
    {
        stats = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        stats.text = playerStats.GetPlayerStats(playerNum).ToString();
    }
}
