using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary>
/// @author Riyad K Rahman <br></br>
/// Determines how a bullet should move and collide with different things
/// </summary>
public class Bounce : MonoBehaviourPunCallbacks
{
    #region Properties
    [Header("Bullet Properties")]
    private GameObject playerOwner;     //a reference to the player who shot/owns the bullet 
    public Rigidbody rb;                //rigidbody component to this bullet gameobject 
    Vector3 lastVelocity;               //the last recorded velocity of the bullet used to calculate Bounce forces 
    private int reflectionCount;        // a counter for the number of bounces 
    public GameObject explosion;            //particle effect for when the bullet explodes 
    private GameObject shield;              //the shield gameObject of the player. (used so bullet passes through player's shield)
    #endregion

    #region Dissolve
    [Header("Bullet Dissolve")]
    private Material mat;                   //rigidbody component to this bullet gameobject (used for dissolving) 
    private Renderer rend;                  //Renderer component to this bullet gameobject (used for dissolving) 
    private bool disableDissovle = false;   //boolean variable to prevent dissolving of the bullet
    private bool beginDissolve = false;    //boolean variable to trigger dissolving of the bullet
    private int frameCounter = 0;        // counts first 15 frames. to be used when destroying frozen bullets
    private float dissolveRate = 1;       //the rate at which the bullet dissolves 
    #endregion



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Start is called before the first frame update
    /// destroys the GameObject 20 seconds after its been fired, 
    /// freezes Y position so that bullet does not fly up or sink down
    /// resets material dissolve value to 1 
    /// </summary>
    void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        Destroy(this.gameObject, 20f);
        rend = GetComponent<Renderer>();
        mat = rend.material;
        mat.SetFloat("Vector1_F9FC2739", 1);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Handles bullet dissolving animation
    /// </summary>
    public void Dissolve()
    {
        this.GetComponent<Collider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        var dissolve = mat.GetFloat("Vector1_F9FC2739");

        //decrements the dissolve factor every frame
        if (dissolve > -0.9f && disableDissovle == false)
        {
            dissolve -= 0.1f * dissolveRate;
            mat.SetFloat("Vector1_F9FC2739", dissolve);
        }
        else
        {
            disableDissovle = true;
            Destroy(this.gameObject, 0f);
        }

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Update is called once per frame
    /// sets last frame velcity to the current velocity, this is neccesary for <see cref="OnCollisionEnter(Collision)"/> when colliding on a reflective surface
    /// calls <see cref="Dissolve"/> to destroy the bullet
    /// </summary>
    void FixedUpdate()
    {
        lastVelocity = rb.velocity;
        if (beginDissolve == true)
        {
            Dissolve();

        }

        // counts 15 frames
        if (frameCounter < 15) { 
            frameCounter++;
        }
       
        else
        {
            //checks if bullet is stuck. if it is then dissolve it.
            if (rb.velocity.magnitude < 1)
            {
                Dissolve();
            }
        }

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Defines how this gameobject reacts on collision with other objects in the scene
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //++++ Damages the player by 10 hitpoints and triggers explosion effect
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAddOns")
        {
            PlayerManager playerhit = collision.transform.root.gameObject.GetComponent<PlayerManager>();
            float playerhitHealth = playerhit.GetHealth();

            if (playerhitHealth < 11 && playerhitHealth > -10) {

                int killvalue = 0;
                
                //if a different player killed this player then they should get a kill
                if (playerhit.gameObject != playerOwner)
                {
                    killvalue++;
                }
                //if you kill yourself then you lose a kill
                else
                {
                    killvalue--;
                }

                playerOwner.GetComponent<PlayerManager>().AddKill(killvalue,playerhit.photonView.ViewID);
            }

            playerhit.DamagePlayer(10);

            Explode();
        }

        //++++ Bullets dissolve if contact with another bullet
        else if (collision.gameObject.tag == "Bullet")
        {
         
            GameObject.Find("TimeManager").GetComponent<AudioSource>().Play();
            dissolveRate = 0.5f;
            beginDissolve = true;
        }

        //++++ Bullets dissolve if contact with an Enemy
        else if (collision.gameObject.tag == "Enemy")
        {          
            beginDissolve = true;
        }

        //++++triggers activatePowerUp function to send a random powerup to the playwerOwner
        // and dissolves the bullet
        else if (collision.gameObject.tag == "Box")
        {            
            beginDissolve = true;
        }

        //++++bullet dissolves on contact with shield
        else if (collision.gameObject.tag == "Shield")
        {           
            dissolveRate = 0.5f;
            beginDissolve = true;
        }

        //++++bullet explodes on contact with nonColliable objects 
        else if (collision.gameObject.tag == "Non_Collidable")
        {
           
            Explode();
        }


        //+++ else for any other object that is hit, the bullet is reflected
        //++++ the number of bounces is incremented 
        else
        {
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
            reflectionCount++;
            rb.velocity = direction * Mathf.Max(speed, 0f);  //++++ change velocity of rb to new direction
        }

        //checks if max number of bounces has been reached 
        checkDestroy();

        //++++ enable collision on shield after a collision is registered
        Physics.IgnoreCollision(shield.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), false);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// if more than 3 bounces are detected then the bullet is dissolved
    /// </summary>
    private void checkDestroy()
    {
        if (reflectionCount > 3)
        {
            beginDissolve = true;
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// This gameObject is exploded and then disabled on all clients
    /// <seealso cref="PunRPC"/>
    /// </summary>
    [PunRPC]
    public void Explode()
    {
        var Exploded = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(Exploded, 2f);
        this.gameObject.SetActive(false);

    }

   /// <summary>
   /// @author Riyad K Rahman <br></br>
   /// assigns bullet's owner to the player who fired it 
   /// </summary>
   /// <param name="playerid">The player who fired this bullet</param>
   public void SetPlayerID(string playerid)
   {
        playerOwner = GameObject.Find(playerid);
        shield = playerOwner.GetComponent<PlayerPowerupManager>().getShield();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br> 
    /// when the game object is disabled it will destroy itself
    /// </summary>
    public override void OnDisable()
    {
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns>returns the owner of the bullet</returns>
    public GameObject getplayer()
    {
        return playerOwner;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// assigns a number of reflections for this bullet 
    /// </summary>
    /// <param name="reflections">the number of bounces before the bullet dissolves</param>
    public void SetReflections(int reflections)
    {
        reflectionCount = reflections;
    }

}


