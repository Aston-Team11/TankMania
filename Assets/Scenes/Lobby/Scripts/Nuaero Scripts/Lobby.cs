using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// Handles syncing players ready state and managing the countdown timer
/// </summary>
public class Lobby : MonoBehaviourPunCallbacks
{
    private bool ready = false;                                     //used locally to track current player's ready status; 
    [SerializeField] private int readyCount = 0;                    //used to count number of players who are ready
    [SerializeField] GameObject myReadyBtn,myTimer,mySession;       //used to change the ready button text
    private GameObject myplayer,otherplayer;                        //used to identify the current player and other players respectively 
        
    private float totalTime = 10;                                   //the totaltime for the countdown timer
     private bool startCount = false;                               //bool controls when to start the countdown
    private int seconds;                                            // total time in seconds
    private string playerName = "";                                 // the name of the player


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning the player buttons in the correct location on the canvas
    /// </summary>
    private void Start()
    {
        if (!(photonView.IsMine))
        {
            //spawns in each player UI element and assigns a postion based on which player spawned
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                Vector3 pos = new Vector3(-275, -65, 0);
                myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);
                playerName = "Player 2: ";
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, playerName);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3 && PhotonNetwork.LocalPlayer.ActorNumber == 3)
            {
                Vector3 pos = new Vector3(200, 65, 0);
                myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);
                playerName = "Player 3: ";
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, playerName);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 4 && PhotonNetwork.LocalPlayer.ActorNumber == 4)
            {              
                Vector3 pos = new Vector3(200, -65, 0);          
                myplayer =  PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);
                playerName = "Player 4: ";
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, playerName);
            }
        }

        else
        {
            Vector3 pos = new Vector3(-275, 65, 0);
            myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);
            playerName = "Player 1: ";
            photonView.RPC("SyncNames", RpcTarget.AllBuffered,myplayer.GetPhotonView().ViewID, playerName);
        }

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles counting of the timer, loads level when counter is 0
    /// and informs all players and new players that the game session has begun
    /// </summary>
    private void Update()
    {
        if (totalTime > 0 && startCount == true)
        {
            totalTime -= Time.deltaTime;
            seconds = (int)totalTime; 
            myTimer.GetComponent<Text>().text = seconds.ToString();
        }
        else if (totalTime <= 0 && startCount == true)
        {
           startCount = false;
            if (photonView.IsMine)
            {
                mySession.GetComponent<GameSession>().setGameSession();
            }
           PhotonNetwork.LoadLevel(4);
        }
    }  


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// handles resetting the countdown timer to join the game scene
    /// </summary>
    /// <param name="state"></param>
    public void ResetSetStartCount(bool state)
    {
        startCount = state;
        totalTime = 10f;
        myTimer.GetComponent<Text>().text = ((int)totalTime).ToString();
    }

    
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// ready's up player (makes changes in UI) and sync's ready state to all clients
    /// </summary>
    public void Readyup()
    {
        // run on each local machine
        ready = !ready;
        if (!ready)
        {
            myReadyBtn.GetComponent<Text>().text = "READY";
        }
        else
        {
            myReadyBtn.GetComponent<Text>().text = "UNREADY";
        }

        //sent to everyone
        photonView.RPC("SyncReady",RpcTarget.AllBuffered,ready, myplayer.GetPhotonView().ViewID);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    ///  sync the players name across all clients
    /// </summary>
    /// <param name="playerid">the id to identify the player</param>
    /// <param name="playername">the name of the player</param>
    [PunRPC]
    public void SyncNames(int playerid, string playername)
    {
        GameObject player = PhotonView.Find(playerid).gameObject;
        player.transform.GetChild(0).gameObject.GetComponent<Text>().text = playername;
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// handles syncing ready check across all clients 
    /// </summary>
    /// <param name="status">bool to check if a player is ready or not</param>
    /// <param name="playerid">int id to identify the player</param>
    [PunRPC]
    public void SyncReady(bool status, int playerid)
    {
        otherplayer = PhotonView.Find(playerid).gameObject;
        ChangePlayerStatus(status);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// updates the ready status of a player, starts countdown clcok if all players are ready
    /// </summary>
    /// <param name="status">bool to check if a player is ready or not </param>
    private void ChangePlayerStatus(bool status)
    {
        //sets UI elements to unready mode
        if (!status)
        {
            otherplayer.transform.GetChild(3).gameObject.SetActive(true);
            otherplayer.transform.GetChild(2).gameObject.SetActive(false);
            otherplayer.transform.GetChild(1).gameObject.GetComponent<Text>().text = "UNREADY";

            if (readyCount > 0)
            {
                readyCount--;
            }

            //reset timer
            startCount = false;
            totalTime = 10f;
            myTimer.GetComponent<Text>().text = ((int) totalTime).ToString();
        }
        //sets UI elements to ready mode
        else
        {
            otherplayer.transform.GetChild(2).gameObject.SetActive(true);
            otherplayer.transform.GetChild(3).gameObject.SetActive(false);
            otherplayer.transform.GetChild(1).gameObject.GetComponent<Text>().text = "READY";

            if (readyCount < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                readyCount++;
            }

            //load players in game when all players are ready
            if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                startCount = true;
            }
            else
            {
                ResetSetStartCount(false);
            }
        }
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// returns to menu screen
    /// </summary>
    public void quit()
    {
        PhotonNetwork.Disconnect();
        Destroy(GameObject.Find("Launcher"));
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

}
