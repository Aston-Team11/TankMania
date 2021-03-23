using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gamemode : MonoBehaviourPunCallbacks
{
    #region PVE
    [Header("Zombie Spawning")]
    private string Enemy = "zombie";
    [SerializeField] private GameObject ZombieSpawners;
    [SerializeField] private float SpawnTimer = 30f;
    public Transform SpawnPointEnemy;
    #endregion


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when the game check game mode and spawn zombies or change player spawn
    /// </summary>
    public void StartPVE(int gamemode)
    {
        //if pve mode is on spawn the zombies
        if (gamemode == 0)
        {
            SpawnZombie();
        }
        //otherwise deactivate spawners to make game run faster
        else
        {
            ZombieSpawners.SetActive(false);
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// spawns a zombie and sends the players position
    /// </summary>
    private void SpawnZombie()
    {
        if (!(photonView.IsMine)) return;

        StartCoroutine(DelaySpawn());
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// delay zombie spawn by spawntimer amount to allow players to group up 
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(SpawnTimer);

        for (int i = 0; i < 5; i++)
        {
            var zombie = PhotonNetwork.Instantiate(Enemy, SpawnPointEnemy.position, SpawnPointEnemy.rotation);
            //zombie.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().TargetPlayer1();
        }
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Adds to spawners list of targets
    /// </summary>
    /// <param name="playerID"></param>
    public void AddPlayerToSpawner(int playerID)
    {
        PhotonView player = PhotonView.Find(playerID);
        ZombieSpawners.GetComponent<Spawner>().addPlayer(player.gameObject);
    }

}
