using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// @author Balraj Bains <br></br>
/// Handles assignment of text in the gameobject
/// </summary>
public class RoomName : MonoBehaviour
{
    /// <summary>
    /// @author Balraj Bains<br></br>
    /// gets the room name from launcher and assigns it to the text component of this gameobject
    /// </summary>
    private void Awake()
    {
        this.gameObject.GetComponent<Text>().text = "Room Name: " + GameObject.Find("Launcher").GetComponent<Launcher>().GetRoomName();
    }

}
