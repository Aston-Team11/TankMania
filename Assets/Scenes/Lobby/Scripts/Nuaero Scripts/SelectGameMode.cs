using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  handles which gamemode to choose 
/// </summary>
public class SelectGameMode : MonoBehaviourPunCallbacks
{
    private GameObject Launcher;        //the shared gameobject across all clients
    private Text txtgamemode;           // the gamemode text 
    private int gamemode = 0;           //the number of the gamemode 
    private bool gamestate = false;     //the state of the gamemode (there are only 2 gamemodes currently)

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  intialises launcher and txtgamemode variables 
    /// </summary>
    void Start()
    {
        Launcher = GameObject.Find("Launcher");
        txtgamemode = GetComponent<Text>();
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  handles changing the gamemode when the host click the button 
    /// </summary>
    public void OnClick()
    {
        if (!photonView.IsMine) { return;  }
       
        gamestate = !gamestate;
        photonView.RPC("SyncGameState", RpcTarget.AllBuffered, gamestate);

    }


    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  syncs selected gamemode on all clients
    /// </summary>
    [PunRPC]
    private void SyncGameState(bool state)
    {
        switch (state)
        {
            case false:
                gamemode = 0;
                txtgamemode.text = "PVE";
                break;

            case true:
                gamemode = 1;
                txtgamemode.text = "FFA";
                break;

        }

        Launcher.GetComponent<Launcher>().SetGameMode(gamemode);
    }

}
