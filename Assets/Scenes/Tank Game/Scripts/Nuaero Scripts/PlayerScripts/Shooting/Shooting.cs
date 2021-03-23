using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public GameObject theBullet;
    public GameObject top;
   // public Material bulletColour;

    public int bulletSpeed;
    // the player who shot the bullet is the parent
    public GameObject parent;

    public bool shootAble = true;
    public float waitBeforeNextShot = 0.25f;
    public GameObject Shield;

    private AudioSource tank_shootingSound;
    public ParticleSystem muzzleFlash;



    /// <summary>
    /// @author Mumin
    /// play sound when tank shoots
    /// </summary>
    public void Start()
    {
        //AudioSource for the tank shooting 
        tank_shootingSound = GetComponent<AudioSource>();
    }

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
