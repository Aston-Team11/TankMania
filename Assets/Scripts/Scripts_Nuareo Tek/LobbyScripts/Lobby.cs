using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class Lobby : MonoBehaviourPunCallbacks
{
    private bool ready = false;                   //used locally to track current player's ready status; 
    [SerializeField] private int readyCount = 0; //used to count number of players who are ready
    [SerializeField] GameObject myReadyBtn,myTimer;     //used to change the ready button text
    private GameObject myplayer,otherplayer;    //used to identify the current player and other players respectively 

    private float totalTime = 10;
     private bool startCount = false;
    private int seconds;


    // Start is called before the first frame update
    private void Start()
    {
        if (!(photonView.IsMine))
        {
            //spawns in each player UI element and assigns a postion based on which player spawned
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Vector3 pos = new Vector3(-275, -65, 0);
                myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);   
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, "Player 2: ");
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                Vector3 pos = new Vector3(200, 65, 0);
                myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);                         
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, "Player 3: ");
            }
            else
            {              
                Vector3 pos = new Vector3(200, -65, 0);          
                myplayer =  PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);                               
                photonView.RPC("SyncNames", RpcTarget.AllBuffered, myplayer.GetPhotonView().ViewID, "Player 4: ");
            }
        }

        else
        {
      
            Vector3 pos = new Vector3(-275, 65, 0);
            myplayer = PhotonNetwork.Instantiate("PlayerBtn", pos, transform.rotation);         
            photonView.RPC("SyncNames", RpcTarget.AllBuffered,myplayer.GetPhotonView().ViewID,"Player 1: ");
        }

    }

    /// <summary>
    /// @author Riyad K Rahman
    /// Handles timer
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
           PhotonNetwork.LoadLevel(4);
        }

    }

    public void ResetSetStartCount(bool state)
    {
        startCount = state;
        totalTime = 10f;
        myTimer.GetComponent<Text>().text = ((int)totalTime).ToString();
    }

    
    /// <summary>
    /// @author Riyad K Rahman
    /// ready's up player (makes changes in UI)
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
    /// @author Riyad K Rahman
    /// sync names on all clients
    /// </summary>
    /// <param name="playerid">the id to identify the player</param>
    [PunRPC]
    public void SyncNames(int playerid, string message)
    {
        GameObject player = PhotonView.Find(playerid).gameObject;
        player.transform.GetChild(0).gameObject.GetComponent<Text>().text = message;
    }


    /// <summary>
    /// @author Riyad K Rahman
    /// handles cyncing ready check acorss all clients 
    /// </summary>
    /// <param name="status">bool to check if a player is ready or not</param>
    /// <param name="playerid">the id to identify the player</param>
    [PunRPC]
    public void SyncReady(bool status, int playerid)
    {
        otherplayer = PhotonView.Find(playerid).gameObject;
        ChangePlayerStatus(status);
    }

    /// <summary>
    /// @author Riyad K Rahman
    /// updates the status of a player
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
    /// @author Riyad K Rahman
    /// return to menu screen
    /// </summary>
    public void quit()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

}
