using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles when to activate this gameobject
/// </summary>
public class playerbtn : MonoBehaviour
{
    [SerializeField] private GameObject canvas; // the canvas this button should become a child to 

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// if a game session has not already begun, then we place the player button into the canvas 
    /// </summary>
    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameSession") != null)
        {
            this.gameObject.SetActive(false);
  
        }

        else
        {
            canvas = GameObject.Find("Canvas");
            this.gameObject.transform.SetParent(canvas.transform, false);
            canvas.GetComponent<Lobby>().ResetSetStartCount(false); //reset lobby countdown
        }
    }

}
