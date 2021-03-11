using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviourPunCallbacks
{
    
    public string mouseReticle;
    public string Enemy;
    public Transform SpawnPoint, SpawnPointEnemy;
    public GameObject time;
    [SerializeField] private int RespawnTime;
    public int count;
    [SerializeField] private List<GameObject> PlayerLists = new List<GameObject>();
    [SerializeField] private List<int> PlayerPhotonViews = new List<int>();

    [SerializeField] private GameObject MySpawners;
    [SerializeField] private float SpawnTimer;
    [SerializeField] private GameObject[] players;
    private AudioSource bgMusic;

    /// <summary>
    /// @author Riyad K Rahman
    /// when the game starts spawn players and zombies
    /// </summary>
    // Start is called before the first frame update
    private void Start()
    {
        var target = SpawnPlayer();
        bgMusic = GetComponent<AudioSource>();
        //SpawnZombie(target);
    }



    /// <summary>
    /// @author Riyad K Rahman
    /// spawn player, set ID, assign time object so player can control time when powerup is enabled
    /// </summary>
    /// <returns>player transform so that the enemies can find the player </returns>
    private Transform SpawnPlayer()
    {

        GameObject newplayer = new GameObject(); 

        if (!(photonView.IsMine))
        {
            //sets different colour presets for each tank
            if (!(GameObject.Find("Player_2")))
            {
                newplayer = PhotonNetwork.Instantiate("Player_2", SpawnPoint.position, SpawnPoint.rotation);
            }
            else if (!(GameObject.Find("Player_3")))
            {
                newplayer = PhotonNetwork.Instantiate("Player_3", SpawnPoint.position, SpawnPoint.rotation);
            }
            else
            {
                newplayer = PhotonNetwork.Instantiate("Player_4", SpawnPoint.position, SpawnPoint.rotation);
            }
            
            //kills clones 
            photonView.RPC("KillMePlz", RpcTarget.OthersBuffered, newplayer.GetPhotonView().ViewID);
           
        }

        else
        {
            //used for spawning host
            newplayer = PhotonNetwork.Instantiate("Player_1", SpawnPoint.position, SpawnPoint.rotation);
        }


        newplayer.name = newplayer.GetPhotonView().ViewID.ToString();
        newplayer.SendMessage("setTimeObject", time);

        photonView.RPC("SyncLists", RpcTarget.AllBuffered, newplayer.GetPhotonView().ViewID);

        return newplayer.transform;
    }

    /// <summary>
    /// bug fix! 
    /// sometimes when joining, you can join twice thus creating two instances of a player.
    /// This function removes the clone 
    /// </summary>
    /// <param name="playerID">The ID of the original player</param>
    [PunRPC]
    public void KillMePlz (int playerID)
    {
        //gets the id of the clone, which is always 
        int cloneID = playerID - 3;
    
           try
           {
               GameObject clonePlayer = PhotonView.Find(cloneID).gameObject;
               Destroy(clonePlayer);
               PlayerLists.Remove(clonePlayer);
               MySpawners.GetComponent<Spawner>().RemovePlayer(clonePlayer);
           }
           catch (Exception) { }
          
    }


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
    /// <param name="targetPosition"> the player 1 posistion in the world</param>
    private void SpawnZombie(Transform targetPosition)
    {
        if (!(photonView.IsMine)) return;

        StartCoroutine(DelaySpawn(targetPosition));

    }

    IEnumerator DelaySpawn(Transform targetPosition)
    {
        yield return new WaitForSeconds(SpawnTimer);

        for (int i = 0; i < 5; i++)
        {
            var zombie = PhotonNetwork.Instantiate(Enemy, SpawnPointEnemy.position, SpawnPointEnemy.rotation);
            zombie.SetActive(true);
            zombie.name = "zombie" + i;
            zombie.SendMessage("TargetPlayer1");

        }
    }


    public void Update()
    {

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

