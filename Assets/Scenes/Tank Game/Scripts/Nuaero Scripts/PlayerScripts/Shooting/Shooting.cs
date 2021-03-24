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
    public GameObject theBullet;                        //the bullet to be fired 
    public GameObject top;                              //used to get the position and rotation of the tank barrel


    public int bulletSpeed;                            // the speed of the bullet 
    public GameObject parent;                          // the player who shot the bullet 
    public bool ShotgunEnable = false;
    public bool MinigunEnable = false; 
    public float PowerupTime = 4.5f;

    public bool shootAble = true;                      //used to handle reloading 
    public float waitBeforeNextShot = 0.25f;           //the time for reload
    public GameObject Shield;                          // the local player's shield (used to disable collisions)

    private AudioSource tank_shootingSound;            // the bullet shooting sound
    public ParticleSystem muzzleFlash;                 // a muzzleflash particle effect to be instantiated per shot 

    



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
    /// @author Riyad K Rahman <br></br>
    /// checks for player's input on mouse button or left Ctrl key.
    /// and sends a server function <see cref="Shoot"/> to shoot an appropraite bullet
    /// </summary>
    private void Update()
    {
        if (!photonView.IsMine) return;


        if (Input.GetButton("Fire1"))
        {
            if (shootAble && MinigunEnable)
            {
                shootAble = false;
                //AudioSource for the tank shooting 
                tank_shootingSound.Play();
                // send shoot function for every player
                photonView.RPC("Shoot", RpcTarget.All);

                StartCoroutine(MinigunYield());
                StartCoroutine(MinigunDisable());
                muzzleFlash.Play();
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
           
             if (shootAble && ShotgunEnable)
            {
                shootAble = false;
                //AudioSource for the tank shooting 
                tank_shootingSound.Play();
                // send shoot function for every player
                photonView.RPC("Shotgun", RpcTarget.All);

                StartCoroutine(ShootingYield());
                StartCoroutine(ShotgunDisable());
                muzzleFlash.Play();

            }
            else if (shootAble)
            {
                shootAble = false;
                //AudioSource for the tank shooting 
                tank_shootingSound.Play();
                // send shoot function for every player
                photonView.RPC("Shoot", RpcTarget.All);

                StartCoroutine(ShootingYield());
                muzzleFlash.Play();
            }

  
        }

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
    public void Shoot()
    {

        // Quaternion rot = transform.rotation;

        //  Quaternion rotation = 
        // rotation = new vector1(rot.x, rot.y, rot.z + 180);

  
       // float angle = 90 * Mathf.Deg2Rad;
      
       // rot.Set(rot.x, rot.y, angle, 1);

        
        var bullet = Instantiate(theBullet, transform.position, top.transform.rotation );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();


        if (Shield.activeSelf)
        
            {
             Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
             
            }


         rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);


        bullet.SendMessage("SetPlayerID", parent.gameObject.name);

        //!!!! space for fx
    }

    [PunRPC]
    public void Shotgun()
    {
        //posititoning for the vectors of the two new bullets 

        Vector3 leftpos = transform.position;
        Vector3 rightpos = transform.position;
        leftpos.x = leftpos.x + -1f;
        rightpos.x = rightpos.x + 1f;

        //creating the bullets

        var bullet = Instantiate(theBullet, transform.position, top.transform.rotation);
        var bullet2 = Instantiate(theBullet, leftpos, (top.transform.rotation * Quaternion.Euler(0, 45f, 0)));
        var bullet3 = Instantiate(theBullet, rightpos, (top.transform.rotation * Quaternion.Euler(0, 315f, 0)));

        //giving all 3 bullets rigid body properties

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
        Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();

        if (Shield.activeSelf)
        {
             Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
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

    [PunRPC]
    public void Minigun()
    {

        var bullet = Instantiate(theBullet, transform.position, top.transform.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();


        if (Shield.activeSelf)
        {
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
        }


        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);


        bullet.SendMessage("SetPlayerID", parent.gameObject.name);

        //!!!! space for fx
    }

    //method to which enables shotgun mode 
    public void SetShotgun()
    {
        ShotgunEnable = true;
        MinigunEnable = false; 
    }

    public void SetMinigun() 
    {
        MinigunEnable = true;
        ShotgunEnable = false;
    }
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when a player respawns, shooting is set to enabled
    /// </summary>
    public void SetShoot(bool val)
    {
        shootAble = val;

    }

    //timing of how long the powerups last
    IEnumerator ShotgunDisable()
    {
        yield return new WaitForSeconds(PowerupTime);
        ShotgunEnable = false;
    }

    IEnumerator MinigunDisable()
    {
        yield return new WaitForSeconds(PowerupTime);
        MinigunEnable = false;
    }
}
