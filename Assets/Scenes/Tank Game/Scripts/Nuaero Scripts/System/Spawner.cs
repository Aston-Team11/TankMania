using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman , Anil Virk <br></br>
/// Handles spawning of zombies
/// </summary>
public class Spawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private int waveNumber = 0;
    [SerializeField] private int enemySpawnAmount = 0;
    [SerializeField] private int enemiesKilled = 0;


    public GameObject[] spawners;
    public GameObject enemy;
    public GameObject target;
    public int Spawncount = 4;
    private int maxwaveNumber = 10;

    [SerializeField] private List<GameObject> PlayerLists = new List<GameObject>();

    /// <summary>
    /// @author Riyad K Rahman, Anil Virk <br></br>
    /// </summary>
    public void IncrementEnemies()
    {
        enemiesKilled++;

        if (photonView.IsMine){
            photonView.RPC("SyncDeathCount", RpcTarget.OthersBuffered, enemiesKilled);
        }

    }

    /// <summary>
    /// @author Anil Virk <br></br>
    /// </summary>
    public int getWave()
    {
        return waveNumber;
    }

    /// <summary>
    /// @author Anil Virk <br></br>
    /// </summary>
    public int getRemaining()
    {
        return enemySpawnAmount - enemiesKilled;
    }

    /// <summary>
    /// @author Anil Virk <br></br>
    /// </summary>
    void Start()
    {
        //set base values of this class
        waveNumber = 1;
        enemySpawnAmount = 5;
        enemiesKilled = 0;

        spawners = new GameObject[5];

        for(int i = 0; i < spawners.Length; i++)
        {
            spawners[i] = transform.GetChild(i).gameObject;
        }

       
  
    }

    /// <summary>
    /// @author Riyad K Rahman, Anil Virk <br></br>
    /// </summary>
    void Update()
    {
        if (!(photonView.IsMine)) return;

        if(enemiesKilled >= enemySpawnAmount)
        {
            NextWave();
        }

    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    // update stats on everyone's screens
    [PunRPC]
     private void SyncWave(int wave)
      {
        waveNumber = wave;
     }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    // update stats on everyone's screens
    [PunRPC]
    private void SyncDeathCount(int killed)
    {
        enemiesKilled = killed;
    }


    /// <summary>
    /// @author Riyad K Rahman, Anil Virk <br></br>
    /// </summary>
    private void NextWave()
    {
        waveNumber++;
        if(waveNumber < maxwaveNumber)
        {
            enemySpawnAmount += 5;
            enemiesKilled = 0;
            photonView.RPC("SyncWave", RpcTarget.OthersBuffered, waveNumber);

            for (int i = 0; i < enemySpawnAmount; i++)
            {
                SpawnEnemy();
            }
        }
        else
        {
            //ends the game for all players
            photonView.RPC("Endgame", RpcTarget.All);
        }
       
    }
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    [PunRPC]
    private void Endgame()
    {
        PhotonNetwork.LoadLevel("EndGame");
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    private void SpawnEnemy()
    {
        Spawncount++;
        int spawnerID = Random.Range(0, spawners.Length);
        

        randomiseSelection();
        var zombie = PhotonNetwork.Instantiate(enemy.name, spawners[spawnerID].transform.position, spawners[spawnerID].transform.rotation);

        zombie.name = "zombie" + Spawncount; // test


    }



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <param name="player"></param>
    public void addPlayer(GameObject player)
    {
        if (!(PlayerLists.Contains(player)))
        {
            PlayerLists.Add(player);
        }

        if (PlayerLists.Count > 1)
        {
            PlayerLists.Sort(sortFunction);
        }

    }
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    public void RemovePlayer(GameObject player)
    {
        PlayerLists.Remove(player);
    }



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private int sortFunction(GameObject p1, GameObject p2)
    {
        if (p1.GetComponent<PlayerManager>().GetOrder() < p2.GetComponent<PlayerManager>().GetOrder())
        {
            return -1;
        }
        else if (p1.GetComponent<PlayerManager>().GetOrder() > p2.GetComponent<PlayerManager>().GetOrder())
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    /// <summary>
    /// @Riyad K Rahman <br></br>
    /// Choose a random player to target when spawned
    /// </summary>
    public void randomiseSelection()
    {
        photonView.RPC("targetPlayer", RpcTarget.AllBuffered, Random.Range(0, PlayerLists.Count));
        //targetPlayer(Random.Range(0, PlayerLists.Count));
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles spawning of zombies
    /// </summary>
    [PunRPC]
    private void targetPlayer(int result)
    {
        target = PlayerLists[result];
        enemy.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().getPlayers(target.transform);
    }

}
