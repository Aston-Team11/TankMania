using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int order;

    [SerializeField] private float health;            //health of player
    [SerializeField] private GameObject healthB;      //health bar
    [SerializeField] private GameObject explosion;    //explosion particle effect
    private Slo_Motion slow;                            //slomotion powerup
    [SerializeField] private GameObject shield;       //shield powerup

    [SerializeField] private GameObject mouseTarget;  // mouse reticle object
    [SerializeField] private mouseTargetSwivel mouseClass;     // handles swiveling top of tank
    [SerializeField] private PredictTrajectory trajectoryClass;   // handles aiming
    [SerializeField] private Shooting shootClass;     // handles swiveling top of tank


    private bool slo;                   //used to enable/disable slo motion
    private string powerUpType = "";    //stores the current powerup as a string


    [SerializeField] private GameObject MySystem; //handles ending the game

    [SerializeField] private bool poision;  //used to enable/disable poison damage
    [SerializeField] private int isPoision; //used to check if poison damage should be appl

    [SerializeField] private int lives;         // number of player lives 
    [SerializeField] private GameObject Balloon; // used for animating respawn
    private bool respawning = false;            //used for controlling respawn
    private Vector3 descend = new Vector3(0f, -5f, 0f);
    [SerializeField] private GameObject livesText;
    [SerializeField] private GameObject shieldPic, slowMoPic;


    //when player enters smoke, increment posion counter
    //when player leaves/ 9 seconds are up decrement counter 
    //if counter less than 0, counter = 0;
    //whne counter is not 0, posion player.


    public void Start()
    {
        SetName();
        SetOrder(Convert.ToInt32(this.gameObject.name));

        if (photonView.IsMine)
        {
            setMouse();
           MySystem = GameObject.Find("----SYSTEMS----");
           
            ActivateHealth();
        }
    }

    /// <summary>
    /// @author Riyad K Rahman
    /// Sets the players name
    /// </summary>
    public void SetName()
    {
        photonView.RPC("UpdateName", RpcTarget.AllBuffered, this.gameObject.name);
    }


    [PunRPC]
    public void UpdateName(string name)
    {
        this.gameObject.name = name;
    }

  

    public void setMouse()
    {
        if (!photonView.IsMine) return;
        //pass mouseTarget GameObject to appropriate classes
        var mouse = Instantiate(mouseTarget, transform.position, transform.rotation);
        mouseTarget = mouse;
        mouseTarget.SetActive(true);
        mouseClass.SetMouseAim(mouseTarget);
        trajectoryClass.SetMouseAim(mouseTarget);
        
    }


    public void setTimeObject(GameObject time)
    {
        slow = time.GetComponent<Slo_Motion>();
    }

    public void ActivateHealth()
    {
        if (!photonView.IsMine) return;
        healthB.SetActive(true);
    }


    private void Update()
    {
        if (!photonView.IsMine) return;

        if (powerUpType.Contains("SLOMO") && (Input.GetKeyDown(KeyCode.Space)))
        {
            slo = !slo;
            slow.Activate(slo);
        }

        else if (powerUpType.Contains("SHIELD") && (Input.GetKeyDown(KeyCode.Space)))
        {
            // only allow it to be active if it is turned off
            if (!(shield.activeSelf))
            {
                photonView.RPC("spawnShield", RpcTarget.AllBufferedViaServer);
                displayShield(false);
            }
        }

        updateHealth();
        setLivesText();

    }

    //handles respawning and its animation
    private void FixedUpdate()
    {

        //for respawning
        if (respawning == true)
        {
            if (transform.position.y <= 0.5)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                photonView.RPC("deactivateBalloon", RpcTarget.AllBuffered);
                shootClass.SetShoot(true);
                gameObject.GetComponent<PlayerMovement>().SetIsInputEnabled(true);
                respawning = false;
            }
            else
            {
                // freeze co-ords (X and Z axis), then slowly descend down
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                transform.Translate(descend * Time.deltaTime, Space.World);
            }
        }
    }

    [PunRPC]
    public void deactivateBalloon()
    {
        Balloon.SetActive(false);
    }


        [PunRPC]
    public void spawnShield()
    {
        shield.SetActive(true);
        shield.SendMessage("resetFernal", -0.2f);
        powerUpType = "";
    }

        // kill player on 0 health
        private void updateHealth()
         {
            PosionDamage();

            if (health <= 0)
            {
                photonView.RPC("RespawnMe", RpcTarget.AllBuffered);
            }
        }

    /// <summary>
    /// disable this player for all players in the room
    /// </summary>
    [PunRPC]
    public void RespawnMe()
    {
        lives--;
 
        if(lives > 0)
        {
            //instantiate explosion effect 
            var Exploded = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(Exploded, 2f);

            //disable shooting
            shootClass.SetShoot(false);

            //disable reticle
            mouseTarget.SetActive(false);

            //respawn player
            this.gameObject.SetActive(false); 
        }
        else
        {
            //endgame
            MySystem.GetComponent<Manager>().CheckForEndGame();
        }

        
    }

 

    private void OnDisable()
    {
        //enable balloon
        Balloon.SetActive(true);

        //reset health bar 
        healthB.GetComponent<HealthBar>().SetMaxHealth(100);

        //enable bool val
        respawning = true;

        //begin respawn (5 seconds)
        InvokeRepeating("startRespawn", 5f, 100f);

    }

    /// <summary>
    /// @author Riyad K Rahman
    /// re-enable reticle, this player and thier health
    /// </summary>
    private void startRespawn()
    {
        health = 100;
        mouseTarget.SetActive(true);
        
        //fall from above
        Vector3 respawnPoint = this.transform.position;
        respawnPoint.y += 50f;
        transform.position = respawnPoint;
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gameObject.GetComponent<PlayerMovement>().SetIsInputEnabled(false);
        //gameObject.GetComponent<Rigidbody>().drag = 1;
        //gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


        this.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        CancelInvoke();
    }


    /// <summary>
    /// 
    /// </summary>
    public void StatReset()
    {
        health = 1000;
    }



    public void PowerupAttained(string powerType)
    {
        displayShield(false);
        displaySlowMo(false);
        powerUpType = powerType;
        if (powerUpType == "SHIELD") {
            displayShield(true);
        }
        else if(powerUpType == "SLOMO") {
            displaySlowMo(true);
        }
    }

   
    public void DamagePlayer(float dmg)
    {
        //!!!! apply any damage multipliers here?
        if (photonView.IsMine)
        {
            health -= dmg;
            photonView.RPC("shareMyHealth", RpcTarget.AllBuffered, health);
        }
    }

    /// <summary>
    /// informs all players in the room of this player's health
    /// </summary>
    /// <param name="dmg"></param>
    [PunRPC]
    public void shareMyHealth(float h)
    {
        health = h;
        healthB.GetComponent<HealthBar>().SetHealth(health);
    }


    public void PosionDamage()
    {
        if(isPoision > 0)
        {
            DamagePlayer(0.05f);
        }
    }

    public int GetPlayerLives()
    {
        return lives;
    }


    public float GetHealth()
    {
        return health;
    }

    public void SetOrder(int num)
    {
        order = num;
    }

    public int GetOrder()
    {
        return order;
    }

    public GameObject getShield()
    {
        return shield;
    }


    public void Setpoisoned(bool state)
    {
        poision = state;

        if (poision == true)
        {
            isPoision++;
            StartCoroutine(endPoison());
        }
        else if(isPoision > 0)
        {
            isPoision--;
            //StopCoroutine(endPoison());
        }

    }

    public bool GetPoisoned()
    {
        return poision;
    }

    IEnumerator endPoison()
    {
        yield return new WaitForSeconds(9f);
        if (isPoision > 0)
        {
            poision = false;
            isPoision --;
        }
    }

    public void setLivesText() {
        livesText.GetComponent<Text>().text = lives.ToString();
    }

    public void displayShield(bool state) {
        shieldPic.SetActive(state);
    }

    public void displaySlowMo(bool state) {
        slowMoPic.SetActive(state);
    }


}


