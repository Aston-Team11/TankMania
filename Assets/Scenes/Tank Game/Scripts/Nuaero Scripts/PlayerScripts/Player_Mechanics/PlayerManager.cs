using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles syncing of this player to other machines and  keeps track of health 
/// </summary>
public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region System
    [Header("System and syncing")]
    [SerializeField] private int order;                             //used to sort players in lists (for zombie spawner targeting)
    private int gameMode;                                          //the current gamemode selceted affects which UI elements are displayed 
    [SerializeField] private GameObject MySystem;                  //handles ending the game
    private InGameMenusManager SharedStats;                        
    [SerializeField] private GameObject pve, ffa;                   //two different UI setups
    [SerializeField] private Text ffaKills;                         //killCount in UI
    private GameObject SpawnPoints;                                 //holds different spawn points a player can spawn from 
    #endregion

    #region Health
    [Header("Health Management")]
    [SerializeField] private float health;                          //health of player
    [SerializeField] private GameObject healthB;                    //health bar
    [SerializeField] private bool poision;                      //used to enable/disable poison damage
    [SerializeField] private int isPoisionCount;                     //used to check if poison damage should be applied
    [SerializeField] private GameObject explosion;                   //explosion particle effect
    #endregion

    #region Aiming
    [Header("Setting Aim")]
    [SerializeField] private GameObject mouseTarget;                // mouse reticle object
    [SerializeField] private mouseTargetSwivel mouseClass;          // handles swiveling top of tank
    [SerializeField] private PredictTrajectory trajectoryClass;   // handles aiming
    [SerializeField] private Shooting shootClass;               // handles swiveling top of tank
    #endregion 

    #region Respawn
    [Header("Respawning")]
    [SerializeField] private int lives;                         // number of player lives 
    [SerializeField] private GameObject Balloon;                // used for animating respawn
    private bool respawning = false;                            //used for controlling respawn
    private Vector3 descend = new Vector3(0f, -5f, 0f);
    [SerializeField] private GameObject livesText;
    #endregion

    #region Kills tracking
    [Header("KillCounts")]
    [SerializeField] private int killCount;                        //number of kills of other players 
    private int maxkills = 10;                                     //number of kills needed to end the game 
    private int previousID, KillCoroutinecount;
    #endregion


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Intialses important variables when this player is first spawned 
    /// </summary>
    public void Start()
    {
        SetName();
        SetOrder(PhotonNetwork.LocalPlayer.ActorNumber);

        if (photonView.IsMine)
        {
            setMouse();
           MySystem = GameObject.Find("----SYSTEMS----");
            SharedStats = GameObject.FindGameObjectWithTag("SharedStats").GetComponent<InGameMenusManager>();
            ActivateHealth();
        }

    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// setup apropriate UI for the gamemode 
    /// </summary>
    /// <param name="gamemode">the current gamemode</param>
    public void GameModeSetup(int gamemode)
    {
        if (photonView.IsMine)
        {
            gameMode = gamemode;

            switch (gameMode)
            {
                case 0:
                    pve.SetActive(true);
                    break;

                case 1:
                    ffa.SetActive(true);
                    lives = 1000;
                    break;

                default:
                    print("No Gamemode Selected");
                    break;
            }
        }
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Sets the players name
    /// </summary>
    public void SetName()
    {
        this.gameObject.name = photonView.ViewID.ToString();
      
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  instantiates (spawn in) the mouse reticle, and assign it to the player's targetting systems 
    /// </summary>
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

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  Activates the health bar on the player's UI
    /// </summary>
    public void ActivateHealth()
    {
        if (!photonView.IsMine) return;
        healthB.SetActive(true);
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    /// checks player's health every frame 
    /// </summary>
    private void Update()
    {
        if (!photonView.IsMine) return;

        updateHealth();
        setLivesText();

    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  handles respawning and its animation
    /// </summary>
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

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  deactivate the balloon gameobject across all clients
    /// </summary>
    [PunRPC]
    public void deactivateBalloon()
    {
        Balloon.SetActive(false);
    }


    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  applies posion damage if applicable, else if player's health is 0 then starts a respawn
    /// </summary>
    private void updateHealth()
     {
        PosionDamage();

        if (health <= 0)
        {
            photonView.RPC("RespawnMe", RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    /// disable this player for all players in the room and update life count across all clients 
    /// </summary>
    [PunRPC]
    public void RespawnMe()
    {
        lives--;
        if (photonView.IsMine )
        {
            //deactivate all powerups for shooting
            //shootClass.ResetPowerups();
            //disable all icons and powerups in powerups manager
            var powerups = GetComponent<PlayerPowerupManager>();
            powerups.PowerupAttained("");
            powerups.displayShotgun(false);
            powerups.displayMinigun(false);

            //update life across all clients only if this player is owned by this machine 
            if (gameMode == 0)
            {
                SharedStats.ChangeLives(order - 1, lives);
            }

        }

        if (lives > 0)
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
            MySystem.GetComponent<Manager>().BeginEndGame();
        }

        
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  resets health stats and starts a respawn
    /// </summary>
    public override void OnDisable()
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
    /// @author Riyad K Rahman <br></br>
    /// re-enable mouse reticle, this player gameobject and thier health
    /// </summary>
    private void startRespawn()
    {
        health = 100;
        mouseTarget.SetActive(true);

        Vector3 respawnPoint = this.transform.position;

        // if the game mode is not FFA then spawn player on thier current position
        if (SpawnPoints != null && photonView.IsMine)
        {
            int result = UnityEngine.Random.Range(0, 4);
            respawnPoint = SpawnPoints.transform.GetChild(result).transform.position;
        }

        //fall from above
        respawnPoint.y += 50f;
        transform.position = respawnPoint;
        gameObject.GetComponent<PlayerMovement>().SetIsInputEnabled(false);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


        this.gameObject.SetActive(true);
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  stops calling respawn function, removes poison stats
    /// </summary>
    public override void OnEnable()
    {
        CancelInvoke();
        poision = false;
        isPoisionCount = 0;
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  applies damage to the player's health and sync's this across all clients.
    ///  this is so only the local player does the damage calculations for itself. 
    ///  Since lag can cause unexpected syncing issues it's best you only recieve damage if you see the event on your machine 
    /// </summary>
    /// <param name="dmg">the amount of damage the player should recieve</param>
    public void DamagePlayer(float dmg)
    {
        //!!!! apply any damage multipliers here
        if (photonView.IsMine)
        {
            health -= dmg;
            photonView.RPC("shareMyHealth", RpcTarget.AllBuffered, health);
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// informs all players in the room of this player's health
    /// </summary>
    /// <param name="dmg"></param>
    [PunRPC]
    public void shareMyHealth(float h)
    {
        health = h;
        healthB.GetComponent<HealthBar>().SetHealth(health);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// if there is at least 1 poisonCount then damage will be applied 
    /// </summary>
    private void PosionDamage()
    {
        if(isPoisionCount > 0)
        {
            DamagePlayer(0.05f);
        }
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  handles poison AOE damage. the counter <see cref="isPoisionCount"/> is used to identify whether the player is in multiple radiation clouds or not 
    ///  the <see cref="isPoisionCount"/>  is decremented after 9 seconds via the coroutine <see cref="endPoison"/>.
    ///  this is because when the radition cloud is destroyed there is no way to notify the player that he should no longer be poisoned
    ///  the player must track thier poison state locally.
    /// </summary>
    /// <param name="state">bool to determine if the player is being poisioned </param>
    public void Setpoisoned(bool state)
    {
        poision = state;

        if (poision == true)
        {
            isPoisionCount++;
            StartCoroutine(endPoison());
        }
        else if (isPoisionCount > 0)
        {
            isPoisionCount--;
        }

    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    /// the <see cref="isPoisionCount"/>  is decremented after 9 seconds
    /// </summary>
    /// <returns>watis for 9 seconds before executing the rest of the function</returns>
    IEnumerator endPoison()
    {
        yield return new WaitForSeconds(9f);
        if (isPoisionCount > 0)
        {
            poision = false;
            isPoisionCount--;
        }
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// this method is used to validate the kill before adding it to this player.
    /// the kill is checked for possible double kill
    /// </summary>
    /// <param name="val"></param>
    /// <param name="playerID">the reference to the player killed </param>
    public void AddKill(int val, int playerID)
    {
        if (!photonView.IsMine || gameMode == 0) { return; }

        if(KillCoroutinecount < 5 && previousID != playerID)
        {
            StartCoroutine(SyncKills(val));
            KillCoroutinecount++;
        }

        previousID = playerID;

    }



    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// increments kill counter and updates UI
    /// when <see cref="maxkills"/> has been reached then the game will end 
    /// </summary>
    /// <param name="val">the value the kills should be incremented by</param>
    /// <returns>waits half a second before executing </returns>
    private IEnumerator SyncKills(int val)
    {
        yield return new WaitForSeconds(0.5f);
        
        killCount += val;
        SharedStats.AddKills(order - 1, killCount);
        ffaKills.text = killCount.ToString();

        if (killCount >= maxkills)
        {
            //endgame
            MySystem.GetComponent<Manager>().BeginEndGame();
        }

        //reset the values for the if conditions
        KillCoroutinecount = 0;
        previousID = 0;
    }


    /// <summary>
    ///  @author Lerai Foulkes <br></br>
    ///  incrments health by its parameter, if it does not go past max health 
    /// </summary>
    /// <param name="inc">the value to increment the health by </param>
    public void AddHealth(float inc)
    {
        //if increment wont go past max health then apply it
        if (photonView.IsMine)
            if(!(health + inc > 100))
            {
                health += inc;
                photonView.RPC("shareMyHealth", RpcTarget.AllBuffered, health);
            }
            else
            {
                health = 100;
                photonView.RPC("shareMyHealth", RpcTarget.AllBuffered, health);
            }
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// 
    /// </summary>
    public void AddLife()
    {
        if (photonView.IsMine)
        {
            lives = lives + 1;
            SharedStats.ChangeLives(order - 1, lives);
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

    public bool GetPoisoned()
    {
        return poision;
    }



    public void setLivesText() {
        livesText.GetComponent<Text>().text = lives.ToString();
    }

   
    public int GetKill()
    {
        return killCount;
    }

    public void SetSpawners(GameObject playerSpawn)
    {
        SpawnPoints = playerSpawn;
    }

    public GameObject GetFFACanvas()
    {
       return ffa ;
    }

    public GameObject GetPVECanvas()
    {
        return pve;
    }

    public Shooting GetShootClass() {
        return shootClass;
    }

    public InGameMenusManager GetSharedStats()
    {
        return SharedStats;
    }
}



