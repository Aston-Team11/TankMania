using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// @author Riyad K Rahman , Anil Virk <br></br>
/// Handles spawning of zombies and intialising a target for newly spawned zombies 
/// </summary>
public class Spawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private int waveNumber = 0;                //current wave number 
    [SerializeField] private int enemySpawnAmount = 0;          //number of enemies which should be spawned 
    [SerializeField] private int enemiesKilled = 0;             //number of enemies killed  

    public GameObject[] spawners;                               //spawnpoints of the zombies
    public GameObject enemy;                                    //the zombie gameobject
    public GameObject target;                                   //the target player for the zombies 
    public int Spawncount = 4;                                  // the number of zombies currently spawned 
    private int maxwaveNumber = 10;                             //the max wave count until the game ends 
    private int numberOfPlayersOffset = 0;                                     //changes the spawn amount depending on how many players there are 

    [SerializeField] private List<GameObject> PlayerLists = new List<GameObject>();  // a list of players 
    [SerializeField] private HashSet<GameObject> nextWavePlayerLists = new HashSet<GameObject>(); // a new list of players 

    /// <summary>
    /// @author Riyad K Rahman, Anil Virk <br></br>
    /// increment enemy killcount and sync's across all clients 
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
    /// intialises variables to thier default value 
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
    /// if the zombies in the current wave have all been killed then the next wave of zombies spawn
    /// </summary>
    void Update()
    {
        if (!(photonView.IsMine)) return;

        if (enemiesKilled >= enemySpawnAmount)
        {
            numberOfPlayersOffset = PhotonNetwork.CurrentRoom.PlayerCount;
            
            //reset the player lists after each wave
            for(int i = 0; i<PlayerLists.Count; i++)
            {
                if (PlayerLists[i] == null)
                {
                    PlayerLists.RemoveAt(i);
                   // nextWavePlayerLists.Add(PlayerLists[i]);
                }

            }
            
            NextWave();

        }
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// syncs wave number across all clients
    /// </summary>
    [PunRPC]
     private void SyncWave(int wave)
      {
        waveNumber = wave;
     }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// syncs zombie's killed across all clients
    /// </summary>
    [PunRPC]
    private void SyncDeathCount(int killed)
    {
        enemiesKilled = killed;
    }


    /// <summary>
    /// @author Riyad K Rahman, Anil Virk <br></br>
    /// spawns in next wave of zombies, an extra zombie is spawned for every player 
    /// </summary>
    private void NextWave()
    {
        waveNumber++;
        if(waveNumber < maxwaveNumber)
        {
            enemySpawnAmount += (5 + numberOfPlayersOffset);
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
    /// Handles ending the game for all clients
    /// </summary>
    [PunRPC]
    private void Endgame()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Endgame", LoadSceneMode.Single);
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
    /// adds a player to the playerlist 
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
    /// removes player from the playerlist
    /// </summary>
    public void RemovePlayer(GameObject player)
    {
        PlayerLists.Remove(player);
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// sorts players based on thier order number 
    /// </summary>
    /// <param name="p1">player gameobject</param>
    /// <param name="p2">player gameobject</param>
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
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles targeting a player to newly spawned zombies 
    /// </summary>
    [PunRPC]
    private void targetPlayer(int result)
    {
        target = PlayerLists[result];
       enemy.GetComponent<AICharacterControl>().GetPlayers(target.transform);
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

}
