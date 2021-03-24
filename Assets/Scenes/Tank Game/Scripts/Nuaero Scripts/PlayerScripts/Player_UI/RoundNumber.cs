using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  @author Riyad K Rahman, Christian Corfield <br></br>
///  gets the appropriate data for the roundnumber and displays this on the player's UI
/// </summary>
public class RoundNumber : MonoBehaviour
{

    private Text roundNumber;                           //the round number Text component
    private int wave;                                   // the wave nubmer 
    private GameObject spawnerObject;                   // the spawner object which holds the data of the current round number 

    /// <summary>
    /// @author Riyad K Rahman, Christian Corfield <br></br>
    /// intialses the spawnerObject and round nubmer variables
    /// </summary>
    void Start()
    {
        spawnerObject = GameObject.Find("Zombie Spawners");
        roundNumber = GetComponent<Text>();
    }

    /// <summary>
    /// @author Riyad K Rahman, Christian Corfield <br></br>
    /// changes the text to the current wave number in the spawner object 
    /// </summary>
    void Update()
    {
        wave = spawnerObject.GetComponent<Spawner>().getWave();
        roundNumber.text = wave.ToString();
    }
}
