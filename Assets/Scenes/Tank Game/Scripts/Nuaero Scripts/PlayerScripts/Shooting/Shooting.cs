using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// Handles shooting mechanic of the player (how bullets should be instantiated)
/// </summary>
public class Shooting : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject theBullet;                        //the bullet to be fired 
    [SerializeField] private GameObject top;                              //used to get the position and rotation of the tank barrel


    [SerializeField] private int bulletSpeed;                            // the speed of the bullet 
    [SerializeField] private GameObject parent;                          // the player who shot the bullet 

    [SerializeField] private bool shootAble = true;                      //used to handle reloading 
    private float waitBeforeNextShot = 0.25f;           //the time for reload
    [SerializeField] private GameObject Shield;                          // the local player's shield (used to disable collisions)

    private AudioSource tank_shootingSound;            // the bullet shooting sound
    public ParticleSystem muzzleFlash;                 // a muzzleflash particle effect to be instantiated per shot 

    [SerializeField] private bool ShotgunEnable = false;
    [SerializeField] private bool MinigunEnable = false;
    [SerializeField] private float PowerupTime = 15f;
    [SerializeField] private Transform fpRight, fpLeft;
    [SerializeField] private GameObject shotgunPic, minigunPic;


    /// <summary>
    /// @author Mumin
    /// play sound when tank shoots
    /// </summary>
    public void Start()
    {
        //AudioSource for the tank shooting 
        tank_shootingSound = GetComponent<AudioSource>();
    }

    /// <summary>
    /// @author Riyad K Rahman, Lerai Foulkes <br></br>
    /// checks for player's input on mouse button or left Ctrl key.
    /// and sends a server function <see cref="Shoot"/> to shoot an appropraite bullet
    /// </summary>
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (shootAble && ShotgunEnable)
            {
                shootAble = false;
                // send shoot function for every player
                photonView.RPC("Shotgun", RpcTarget.All);

                StartCoroutine(ShootingYield());
                muzzleFlash.Play();

            }

            else if (shootAble && MinigunEnable)
            {
                shootAble = false;
                //triggers mingun shooting every 0.1 seconds 
                InvokeRepeating("MinigunShoot", 0f, 0.15f);
                StartCoroutine(MinigunDisable());
                muzzleFlash.Play();
            }

            else if (shootAble)
            {
                shootAble = false;
                // send shoot function for every player
                photonView.RPC("Shoot", RpcTarget.All, false);
      
                StartCoroutine(ShootingYield());
                muzzleFlash.Play();
            }
        }

        //if a player lifts off shoot button then cancel mingun shooting 
        else if (Input.GetButtonUp("Fire1") && MinigunEnable)
        {
            CancelInvoke("MinigunShoot");
            shootAble = true;
        }
    }

    /// <summary>
    /// @Riyad K Rahman, Lerai Foulkes <br></br>
    /// send shoot function for every player
    /// </summary>
    private void MinigunShoot()
    {
        
        photonView.RPC("Shoot", RpcTarget.All, true);
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns>waits for a reload time, then sets <see cref="shootAble"/> to true</returns>
    IEnumerator ShootingYield()
    {
        yield return new WaitForSeconds(waitBeforeNextShot);
        shootAble = true;
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// </summary>
    /// <returns>waits for a reload time, then sets <see cref="shootAble"/> to true</returns>
    IEnumerator MinigunYield()
    {
        yield return new WaitForSeconds(0.1f);
        shootAble = true;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    ///  bullets are instantiated only by the person who shoots 
    /// </summary>
    [PunRPC]
    public void Shoot(bool minigunPower)
    {
        //AudioSource for the tank shooting 
        tank_shootingSound.Play();

        var bullet = Instantiate(theBullet, transform.position, transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (Shield.activeSelf)
        {
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
        }

        //if the powerup is enabled then we disable bullet bounce 
        if (minigunPower)
        {
            bullet.GetComponent<Bounce>().SetReflections(3);
        }

        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);

        bullet.SendMessage("SetPlayerID", parent.gameObject.name);
   
       //!!! space for fx
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// handles spawning of shotgun bullets 
    /// </summary>
    [PunRPC]
    public void Shotgun()
    {
        //AudioSource for the tank shooting 
        tank_shootingSound.Play();

        //creating the bullets
        var bullet = Instantiate(theBullet, transform.position, top.transform.rotation);
        var bullet2 = Instantiate(theBullet, fpLeft.position, top.transform.rotation);
        var bullet3 = Instantiate(theBullet, fpRight.position, top.transform.rotation);

        //disables bullet bounce for the three bullets
        bullet.GetComponent<Bounce>().SetReflections(3);
        bullet2.GetComponent<Bounce>().SetReflections(3);
        bullet3.GetComponent<Bounce>().SetReflections(3);

        //giving all 3 bullets rigid body properties

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
        Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();

        //pass through shield
        if (Shield.activeSelf)
        {
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
            Physics.IgnoreCollision(bullet2.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
            Physics.IgnoreCollision(bullet3.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
        }

        //adding the movement of all the bullets 

        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        rb2.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        rb3.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);

        bullet.SendMessage("SetPlayerID", parent.gameObject.name);
        bullet2.SendMessage("SetPlayerID", parent.gameObject.name);
        bullet3.SendMessage("SetPlayerID", parent.gameObject.name);

        //!!!! space for fx
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// disables shotgun powerup after a certian timed has passed 
    /// </summary>

    IEnumerator ShotgunDisable()
    {
        yield return new WaitForSeconds(PowerupTime);
        displayShotgun(false);
        ShotgunEnable = false;
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// disables minigun powerup after a certian timed has passed 
    /// </summary>
    IEnumerator MinigunDisable()
    {
        yield return new WaitForSeconds(PowerupTime);
        CancelInvoke("MinigunShoot");
        displayMinigun(false);
        MinigunEnable = false;
        shootAble = true;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when a player respawns, shooting is set to enabled
    /// </summary>
    public void SetShoot(bool val)
    {
       
        shootAble = val;
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// enables shotgun, disables other mdoes 
    /// </summary>
    public void SetShotgun()
    {
        StopAllCoroutines();
        shootAble = true;
        ShotgunEnable = true;
        displayShotgun(true);
        MinigunEnable = false;
        StartCoroutine(ShotgunDisable());
    }

    /// <summary>
    /// @author Lerai Foulkes <br></br>
    /// enables minigun, disables other mdoes 
    /// </summary>
    public void SetMinigun()
    {
        StopAllCoroutines();
        shootAble = true;
        MinigunEnable = true;
        displayMinigun(true);
        ShotgunEnable = false;
        StartCoroutine(MinigunDisable());
    }
    public void displayShotgun(bool state)
    {
        shotgunPic.SetActive(state);
    }

    public void displayMinigun(bool state)
    {
        minigunPic.SetActive(state);
    }
}
