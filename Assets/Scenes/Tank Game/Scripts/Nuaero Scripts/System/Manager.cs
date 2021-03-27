using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  handles spawning players and ending the game across all clients 
/// </summary>
public class Manager : MonoBehaviourPunCallbacks
{
    
    #region Players
    [Header("Player attributes")]
    public GameObject[] playerSpawns;
    public GameObject time;
    #endregion

    #region Gamemode
    [Header("Gamemode attributes")]
    [SerializeField] private int gameMode;                  // the currently active gamemode
    [SerializeField] private Gamemode gmclass;             // the gamemode class, which handles enabling and sending appropraite data to objects of the associated gamemode
    [SerializeField] private PowerupSpawner PwrUpSpawnclass;             // the powerupspawner class, which handles spawning powerups and which powerups should be enabled 
    private Launcher launcher;                             // the shared gameobject across all clients. holds data about which gamemode was selected 
    #endregion

    public GameObject mycam;                                // the shared scene camera

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

        PwrUpSpawnclass.gameMode = gameMode;
        StartCoroutine(SpawnPlayer());
    }



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// spawn player, set ID, assign time object so player can control time when powerup is enabled
    /// </summary>
    /// <returns>player transform so that the enemies can find the player </returns>
    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        mycam.SetActive(false);

        GameObject newplayer = new GameObject();


        if (!(photonView.IsMine))
        {
            //sets different colour presets for each tank
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                newplayer = PhotonNetwork.Instantiate("Player_2", playerSpawns[gameMode].transform.GetChild(1).transform.position, playerSpawns[gameMode].transform.GetChild(1).transform.rotation);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3 && PhotonNetwork.LocalPlayer.ActorNumber == 3)
            {
                newplayer = PhotonNetwork.Instantiate("Player_3", playerSpawns[gameMode].transform.GetChild(2).transform.position, playerSpawns[gameMode].transform.GetChild(2).transform.rotation);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 4 && PhotonNetwork.LocalPlayer.ActorNumber == 4)
            {
                newplayer = PhotonNetwork.Instantiate("Player_4", playerSpawns[gameMode].transform.GetChild(3).transform.position, playerSpawns[gameMode].transform.GetChild(3).transform.rotation);
            }
        }

        else
        {
            //used for spawning host
            newplayer = PhotonNetwork.Instantiate("Player_1", playerSpawns[gameMode].transform.GetChild(0).transform.position, playerSpawns[gameMode].transform.GetChild(0).transform.rotation);
           
            //used to inform players that a game session has already begun
            PhotonNetwork.Instantiate("GameInSession", transform.position, transform.rotation);

        }

        //newplayer.name = newplayer.GetPhotonView().ViewID.ToString();
        newplayer.SendMessage("setTimeObject", time);
        newplayer.GetComponent<PlayerManager>().GameModeSetup(gameMode);

        //sync zombie spawner if gamemode is currently pve 
        if (gameMode == 0)
        {
            photonView.RPC("SyncLists", RpcTarget.AllBuffered, newplayer.GetPhotonView().ViewID);
        }

        // set ffa spawnpoints if gamemode is currently ffa
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

