using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  handles spawning of powerupboxes 
/// </summary>
public class PowerupSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject powerBox;           //the powerupbox gameobject to be spawned
    [SerializeField] private int BoxCount = 0;              // the number of boxes currently spawned 

    [SerializeField] private GameObject[] spawners;         //the possible spawn locations of a box 
    private float myTotalTime = 0f;                         // total time in float
    private int seconds = 0;                                // total time in seconds
    private bool spawning = false;                          // bool state which controls when to spawn a powerupbox
    private int spawnTime = 20;                             // the interval at which a new set of boxes will spawn

    private int playerCount = 0;                            //the number of players in the room


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// sets the playerCount
    /// </summary>
    private void Start()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Spawns boxes in
    /// </summary>
    private void Update()
    {
        //only the master client handles the time management
        if (!photonView.IsMine) return;

        myTotalTime += Time.deltaTime;
        seconds = (int) myTotalTime;

        //checks if there is already enough boxes per player 
        if (BoxCount < playerCount)
        {

            if (spawning == false && (seconds % spawnTime == 0))
            {
                spawning = true;

                // spawn 1 powerup per player
                for (int i = 0; i < playerCount; i++)
                {
                    int result = Random.Range(0, 20);
                    SpawnPowerup(result);
                }
            }

            //re-enable powerup spawning after 1 second 
            if (seconds != 0 && (seconds % (spawnTime + 1) == 0))
            {
               
                spawning = false;
            }
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Spawns a powerup box in game
    /// </summary>
    /// <param name="spawnerId">used to identify which spawner to use</param>
    private void SpawnPowerup(int spawnerId)
    {
       var box = PhotonNetwork.Instantiate(powerBox.name, spawners[spawnerId].transform.position, transform.rotation);
       box.GetComponent<PowerUp>().SetMySpawner(this.gameObject);
       BoxCount++; 
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// decrements BoxCount
    /// </summary>
    public void decrementBoxCount()
    {
        if(BoxCount > 0)
        {
            BoxCount--;
        }
    }


}
