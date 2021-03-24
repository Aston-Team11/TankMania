using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  @author Christian Corfield <br></br>
///  gets the appropriate data for the roundnumber and displays this on the player's UI
/// </summary>
public class ZombiesRemaining : MonoBehaviour
{
    private Text zombiesRemaining;                      //the number of zombies remaning 

    /// <summary>
    ///  @author Christian Corfield <br></br>
    ///  intialses the current number of zombies
    /// </summary>
    void Start()
    {
        zombiesRemaining = GetComponent<Text>() as Text;
    }

    /// <summary>
    /// @author Riyad K Rahman , Christian Corfield <br></br>
    /// finds all he zombies in the scene and assigns the text in the player's UI for zombies remaining
    /// </summary>
    void Update()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
        zombiesRemaining.text = list.Length.ToString();
    }
}

