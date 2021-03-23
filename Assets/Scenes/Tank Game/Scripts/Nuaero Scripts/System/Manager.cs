using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviourPunCallbacks
{
    
    #region Players
    [Header("Player attributes")]
    public GameObject[] playerSpawns;
    public GameObject time;
    #endregion

    [SerializeField] private int gameMode;
    public GameObject mycam;

    [SerializeField] private Gamemode gmclass;
    private AudioSource bgMusic;
    private Launcher launcher;

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when the game starts spawn the players
    /// </summary>
    private void Start()
    {
        launcher = GameObject.Find("Launcher").GetComponent<Launcher>();
        gameMode = launcher.GetGameMode();

        if (gameMode == 0)
        {
            gmclass.StartPVE(gameMode);
        }
        bgMusic = GetComponent<AudioSource>();

        StartCoroutine(SpawnPlayer());
    }



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// spawn player, set ID, assign time object so player can control time when powerup is enabled
    /// </summary>
    /// <returns>player transform so that the enemies can find the player </returns>
    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(3f);
        mycam.SetActive(false);

        GameObject newplayer = new GameObject(); 

        if (!(photonView.IsMine))
        {
            //sets different colour presets for each tank
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                newplayer = PhotonNetwork.Instantiate("Player_2", playerSpawns[gameMode].transform.GetChild(1).transform.position, playerSpawns[gameMode].transform.GetChild(1).transform.rotation);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                newplayer = PhotonNetwork.Instantiate("Player_1", playerSpawns[gameMode].transform.GetChild(2).transform.position, playerSpawns[gameMode].transform.GetChild(2).transform.rotation);
            }
            else
            {
                newplayer = PhotonNetwork.Instantiate("Player_3", playerSpawns[gameMode].transform.GetChild(3).transform.position, playerSpawns[gameMode].transform.GetChild(3).transform.rotation);
              
            }
        }

        else
        {
            //used for spawning host
            newplayer = PhotonNetwork.Instantiate("Player_1", playerSpawns[gameMode].transform.GetChild(0).transform.position, playerSpawns[gameMode].transform.GetChild(0).transform.rotation);
           
            //used to inform players that a game session has already begun
            PhotonNetwork.Instantiate("GameInSession", transform.position, transform.rotation);

        }

        newplayer.name = newplayer.GetPhotonView().ViewID.ToString();
        newplayer.SendMessage("setTimeObject", time);
        newplayer.GetComponent<PlayerManager>().GameModeSetup(gameMode);

        //sync zombie spawner if gamemode is pve 
        if (gameMode == 0)
        {
            photonView.RPC("SyncLists", RpcTarget.AllBuffered, newplayer.GetPhotonView().ViewID);
        }

        // set spawnpoints if gamemode is ffa
        else if (gameMode == 1)
        {
            newplayer.GetComponent<PlayerManager>().SetSpawners(playerSpawns[gameMode]);
        }

        
    }

    [PunRPC]
    public void SyncLists(int playerID)
    {
        gmclass.AddPlayerToSpawner(playerID);
    }

    public void BeginEndGame()
    {
        photonView.RPC("EndGame", RpcTarget.All);
    }

    [PunRPC]
    public void EndGame()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Endgame", LoadSceneMode.Single);
    }


}

