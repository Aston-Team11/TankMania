using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  disconnects the player from the server
/// </summary>
public class PlayerDisconnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Destroy(GameObject.Find("Launcher"));
        PhotonNetwork.Disconnect();
    }


}
