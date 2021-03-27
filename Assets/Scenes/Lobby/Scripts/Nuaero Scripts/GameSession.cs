using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman, Waheedullah Jan <br></br>
/// Used to inform players that a game session has already begun, and displays an appropraite error screen
/// </summary>
public class GameSession : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool ingame = false;   //the state of game in progress 
    [SerializeField] private GameObject bg, lobby, errorscreen;

    private void Start()
    {
        StartCoroutine(checkJoin());
    }

    /// <summary>
    /// @author Waheedullah Jan <br></br>
    /// if a new player tries to join in-game session they are disconnected
    /// </summary>
    IEnumerator checkJoin()
    {
        yield return new WaitForSeconds(0.1f);

        //check if a game session has already begun. close other screens and display error screen
        if (GameObject.FindGameObjectWithTag("GameSession") != null)
        {
            PhotonNetwork.Disconnect();
            bg.SetActive(false);
            lobby.SetActive(false);
            errorscreen.SetActive(true);

            this.gameObject.SetActive(false);
        }
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
    ///  @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns>current status of the game session</returns>
    public bool getGameSession()
    {
        return ingame;
    }


}
