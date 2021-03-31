using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary>
/// @author Riyad K Rahman <br></br>
/// class handles randomising and sending powerups to a player
/// </summary>
public class PowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject explosion;            // the paricle effect for explosion
    private AudioSource explosion_soundEffect;                 // the explosion sound effect for the power up boxes
    private GameObject mySpawner;                              // the powerup spawner 
    private Vector3 descend = new Vector3(0f, -5f, 0f);        //the rate of descent to be applied when the powerup box spawns in
    private int MaxRange;

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Finds the audio component to play the explosion sound
    /// </summary>
    public void Start()
    {
        explosion_soundEffect = GameObject.FindGameObjectWithTag("PowerUpExplosion").GetComponent<AudioSource>();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// moves the box down until it has reached the floor 
    /// </summary>
    private void FixedUpdate()
    {
        //drop in the box 
        if (transform.position.y > 0.5)
        {
            transform.Translate(descend * Time.deltaTime, Space.World);
        }
        else
        { return; }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when this gameObject collides with a bullet the <see cref="Explode"/> function is called to break the box
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("givePowerUp");
            activatePowerUp(collision.gameObject.GetComponent<Bounce>().getplayer());
            photonView.RPC("Explode", RpcTarget.AllBuffered);
        }

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// This gameObject is exploded and a server message (<see cref="sendDestroy"/> is sent to destroy the box for all players
    /// </summary>
    [PunRPC]
    private void Explode()
    {
        var Exploded = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(Exploded, 2f);
        photonView.RPC("sendDestroy", RpcTarget.AllBufferedViaServer);
        explosion_soundEffect.Play();        //play the explosion sound effect.
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// PUNRPC <see cref="PunRPC"/> is used to identify methods as server methods. Photon uses this function to destroy this gameObject
    /// </summary>
    [PunRPC]
    public void sendDestroy()
    {
        this.gameObject.SetActive(false);
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when the game object is disabled it will destroy itself only on the master client
    /// </summary>
    public override void OnDisable()
    {
        try
        {
            if (photonView.IsMine)
            {
                mySpawner.GetComponent<PowerupSpawner>().decrementBoxCount();
                PhotonNetwork.Destroy(this.photonView);
            }
        }
        catch (MissingReferenceException)
        {
            Debug.Log("the game has ended");
        }

    }

    /// <summary>
    /// @author Riyad K Rahman, Lerai Foulkes <br></br>
    /// 'result' picks a random number 1 or 2. (at the moment this is only 2 powerups, but will be many more in a later date)
    /// A case statement then sends a message to the player <see cref="PlayerManager.PowerupAttained(string)"/> with a string of the type of powerup being sent 
    /// </summary>
    /// <param name="player"> is used to identify which player to send the powerup to</param>
    public void activatePowerUp(GameObject player)
    {
        if (player.activeSelf == false)
        {
            return;
        }

        int result = Random.Range(1, MaxRange);

        switch (result)
        {
            case 1:
                player.SendMessage("PowerupAttained", "SLOMO");
                Debug.Log("SENT SLOMO");
                break;

            case 2:
                player.SendMessage("PowerupAttained", "SHIELD");
                Debug.Log("SENT SHIELD");
                break;

            case 3:
                player.SendMessage("PowerupAttained", "HealthUp");
                Debug.Log("Health up by 10 ");
                break;

            case 4:
                player.SendMessage("PowerupAttained", "Minigun");
                Debug.Log("Minigun Attained");
                break;

            case 5:
                player.SendMessage("PowerupAttained", "Shotgun");
                Debug.Log("Shotgun Attained");
                break;

            case 6:
                player.SendMessage("PowerupAttained", "AdditionalLife");
                Debug.Log("Extra Life");
                break;


            default:
                print("ErROR No PoWEr uP FouNd");
                break;
        }


    }

    public void SetMySpawner(GameObject spawner)
    {
        mySpawner = spawner;
    }

    public void SetMaxRange(int range)
    {
        photonView.RpcSecure("SyncRange", RpcTarget.All, true, range);
    }

    [PunRPC]
    private void SyncRange(int range)
    {
        MaxRange = range;
    }

}
