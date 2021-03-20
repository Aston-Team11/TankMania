using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviourPunCallbacks
{
    
    private string Enemy = "zombie";
    public Transform[] SpawnPoint;
    public Transform SpawnPointEnemy;
    public GameObject time;
    [SerializeField] private int RespawnTime;
    public int count;
    [SerializeField] private List<GameObject> PlayerLists = new List<GameObject>();
    [SerializeField] private List<int> PlayerPhotonViews = new List<int>();

    [SerializeField] private GameObject MySpawners;
    [SerializeField] private float SpawnTimer;
    [SerializeField] private GameObject[] players;
    private AudioSource bgMusic;
    [SerializeField] private bool PVE;


    /// <summary>
    /// @author Riyad K Rahman
    /// when the game starts spawn players and zombies
    /// </summary>
    private void Start()
    {
        SpawnPlayer();
        bgMusic = GetComponent<AudioSource>();
        
        //if pve mode is on spawn the zombies
        if (PVE) 
        { 
            SpawnZombie();
        }
        //otherwise deactivate spawners to make game run faster
        else
        {
            GameObject.Find("Zombie Spawners").SetActive(false);
        }
       
    }



    /// <summary>
    /// @author Riyad K Rahman
    /// spawn player, set ID, assign time object so player can control time when powerup is enabled
    /// </summary>
    /// <returns>player transform so that the enemies can find the player </returns>
    private void SpawnPlayer()
    {

        GameObject newplayer = new GameObject(); 

        if (!(photonView.IsMine))
        {
            //sets different colour presets for each tank
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                newplayer = PhotonNetwork.Instantiate("Player_2", SpawnPoint[1].position, SpawnPoint[1].rotation);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                newplayer = PhotonNetwork.Instantiate("Player_1", SpawnPoint[2].position, SpawnPoint[2].rotation);
            }
            else
            {
                newplayer = PhotonNetwork.Instantiate("Player_3", SpawnPoint[3].position, SpawnPoint[3].rotation);
              
            }
            //kills clones  /// @dead code only remove for final publish
            //  photonView.RPC("KillMePlz", RpcTarget.OthersBuffered, newplayer.GetPhotonView().ViewID);

        }

        else
        {
            //used for spawning host
            newplayer = PhotonNetwork.Instantiate("Player_1", SpawnPoint[0].position, SpawnPoint[0].rotation);
        }

        newplayer.name = newplayer.GetPhotonView().ViewID.ToString();
        newplayer.SendMessage("setTimeObject", time);

        photonView.RPC("SyncLists", RpcTarget.AllBuffered, newplayer.GetPhotonView().ViewID);

        
    }

  // /// <summary>
  // /// @dead code only remove for final publish
  // /// sometimes when joining, you can join twice thus creating two instances of a player.
  // /// This function removes the clone 
  // /// </summary>
  // /// <param name="playerID">The ID of the original player</param>
  // [PunRPC]
  // public void KillMePlz (int playerID)
  // {
  //     //gets the id of the clone, which is always 
  //     int cloneID = playerID - 3;
  // 
  //        try
  //        {
  //            GameObject clonePlayer = PhotonView.Find(cloneID).gameObject;
  //            Destroy(clonePlayer);
  //            PlayerLists.Remove(clonePlayer);
  //            MySpawners.GetComponent<Spawner>().RemovePlayer(clonePlayer);
  //        }
  //        catch (Exception) { }
  //       
  // }


    [PunRPC]
    public void SyncLists(int playerID)
    {
        var player = PhotonView.Find(playerID);
        PlayerLists.Add(player.gameObject);
        MySpawners.GetComponent<Spawner>().addPlayer(player.gameObject);
        //player.gameObject.GetComponent<PlayerManager>().addplayer(player);
    }


    /// <summary>
    /// Adds to spawners list of targets
    /// </summary>
    /// <param name="playerID"></param>
    public void AddPlayerToSpawner(PhotonView playerID)
    {
        MySpawners.GetComponent<Spawner>().addPlayer(playerID.gameObject);
    }



    /// <summary>
    /// @author Riyad K Rahman
    /// spawns a zombie and sends the players position
    /// </summary>
    private void SpawnZombie()
    {
        if (!(photonView.IsMine)) return;

        StartCoroutine(DelaySpawn());

    }

    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(SpawnTimer);

        for (int i = 0; i < 5; i++)
        {
            var zombie = PhotonNetwork.Instantiate(Enemy, SpawnPointEnemy.position, SpawnPointEnemy.rotation);
           // zombie.name = "zombie" + i;
           // zombie.SendMessage("TargetPlayer1");
            zombie.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().TargetPlayer1();
           
        }
    }

    public void CheckForEndGame()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("EndGame");
    }

    [PunRPC]
    public void EndGame()
    {
        SceneManager.LoadScene("Dialogue Scene", LoadSceneMode.Single);

    }


}

