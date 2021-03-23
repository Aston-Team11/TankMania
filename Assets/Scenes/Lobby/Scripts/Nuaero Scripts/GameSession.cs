using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// Used to inform players that a game session has already begun
/// </summary>
public class GameSession : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool ingame = false;   //the state of game in progress 


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// informs other players that the game session has already started 
    /// <seealso cref="PunRPC"/>
    /// </summary>
    [PunRPC]
     public void Ingame()
     {
         ingame = true;
     }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when a game has started this function is caleed to set teh game in session to true;
    /// <seealso cref="PhotonView.RPC(string, Photon.Realtime.Player, object[])"/>
    /// </summary>
    public void setGameSession()
    {
        photonView.RPC("Ingame", RpcTarget.AllBuffered);
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns></returns>
    public bool getGameSession()
    {
        return ingame;
    }


}
