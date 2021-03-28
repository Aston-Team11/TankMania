using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private InGameMenus playerStats;
    [SerializeField] private int playerNum;
   private Text kills;

    // Start is called before the first frame update
    void Start()
    {
        kills = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        kills.text = playerStats.GetPlayerKills(playerNum).ToString();
    }
}
