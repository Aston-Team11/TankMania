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

        if (Input.GetButtonDown("Fire1"))
        {

            if (shootAble)
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

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    ///  bullets are instantiated only by the person who shoots 
    /// </summary>
    [PunRPC]
    public void Shoot()
    {
            Quaternion rot = top.transform.rotation;
            float angle = 90 * Mathf.Deg2Rad;

            rot.Set(rot.x, rot.y, angle, 1);
            var bullet = Instantiate(theBullet, transform.position, rot);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (Shield.activeSelf)
            {
                Physics.IgnoreCollision(bullet.GetComponent<Collider>(), Shield.GetComponent<Collider>(), true);
            }


            rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);

            bullet.SendMessage("SetPlayerID", parent.gameObject.name);
   
        //!!!! space for fx
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// when a player respawns, shooting is set to enabled
    /// </summary>
    public void SetShoot(bool val)
    {
        shootAble = val;
    }



}
