using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles showing a shared player stat (eihter kills or lives) depending on the gamemode 
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private InGameMenusManager SharedplayerStats;
    [SerializeField] private int playerNum;
   private Text stats;  //the number of kills or the nubmer of player lives depending onwhich gamemode is active

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Intialses the text variable with the text componented connected to this gameObject
    /// </summary>
    void Start()
    {
        stats = gameObject.GetComponent<Text>();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// updates the text with values stored in a globally shared gameObject
    /// </summary>
    void Update()
    {
        stats.text = SharedplayerStats.GetPlayerStats(playerNum).ToString();
    }
}
