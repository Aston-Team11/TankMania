using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// @author Riyad K Rahman <br></br>
/// Handles radiation cloud behaviour and how it interacts with players
/// </summary>
public class Radiation : MonoBehaviour
{
    //public float sphereRadius;                      //used to measure the radius of the radiation cloud in Unity Editor
    [SerializeField] private GameObject player;     // a player gameobject that is interacting with the cloud 

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Upon spawn calls a Coroutine to begin self destruction
    /// </summary>
    private void Start()
    {
          StartCoroutine(SelfDestruction());
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns> waits 9 seconds (for the cloud to dissipate) then destroy this gameObject </returns>
    IEnumerator SelfDestruction()
    {
        yield return new WaitForSeconds(9f);
        Destroy(this.gameObject);

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// When any collider enters the collider of the cloud, it will check if it is a player
    /// then apply the poision effect to that player
    /// </summary>
    /// <param name="playerCollider">The collider of the player GameObject</param>
    private void OnTriggerEnter(Collider playerCollider)
    {

        if (playerCollider.gameObject.CompareTag("Player"))
        {
            player = playerCollider.gameObject;
            player.GetComponent<PlayerManager>().Setpoisoned(true);
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// When any collider exits the collider of the cloud, it will check if it is a player
    /// then remove the poision effect to that player
    /// </summary>
    /// <param name="playerCollider">The collider of the player GameObject</param>
    private void OnTriggerExit(Collider playerCollider)
    {
        if (playerCollider.gameObject.CompareTag("Player") )
        {
            player = playerCollider.gameObject;
           player.GetComponent<PlayerManager>().Setpoisoned(false);
        }
    }

 }
