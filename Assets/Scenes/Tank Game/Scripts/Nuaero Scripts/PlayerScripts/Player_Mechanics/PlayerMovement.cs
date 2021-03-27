using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  handles player movement of the main tank body and drifting calculations 
/// </summary>
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private Rigidbody rb;                               //the rigidbody component of the player 

    public float acceleration, torqueForce;             // the acceleration and drifting force to be applied to the tank 
    private float defaultRotate, defaultSpeed;          // values to adjust acceleration and drifting force  when time is slowed
    private bool IsInputEnabled = true;                 // a boolean to check if any user input should be applied

    private float steeringOffset = 15;                         //an offset to be applied when rotating the player 
    public Vector2 direction;                                 // the direction of the player's inputs 
  
    public GameObject parentCam;                            //the camera of the player
    [SerializeField] private AudioSource engine;           //the engine sound which adjusts when the player moves 
    [SerializeField] private float audioOffset;            //the offset at which the pitch of the engine sound should be adjusted (mimics reving sound)

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  initialses default values and sets the player's camera to look at this player 
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            defaultSpeed = acceleration;
            defaultRotate = torqueForce;
            var cam = Instantiate(parentCam, parentCam.transform.position, parentCam.transform.rotation);
            
            if (parentCam)
            {
                try
                {
                    cam.SetActive(photonView.IsMine);
                    cam.SendMessage("getPlayer", gameObject);
                }

                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
            }
        }
    }


    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  gets player's inputs every frame 
    /// </summary>
    private void Update()
    {
        // check if player view is this player
        if (!photonView.IsMine) return;
        if(!IsInputEnabled) return;
        direction.x = Input.GetAxisRaw("Vertical");
        direction.y = Input.GetAxisRaw("Horizontal") * steeringOffset;
      
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  handles physics applied to the tank body 
    /// </summary>
    private void FixedUpdate()
    {
        // check if player view is this player
        if (!photonView.IsMine) return;

        // apply forward/backward force dependent on user input 
        rb.AddForce(transform.forward * direction.x * acceleration);

        // engine revving sound 
        float result = Mathf.Lerp(0f, rb.velocity.magnitude * 1.09f , 0.1f);
        engine.pitch = result + audioOffset;


        //rotation
        rb.AddTorque(transform.up * torqueForce * direction.y);

        //check if time has been changed, adjust acceleration if time is slowed
        acceleration = (Time.timeScale != 1) ? defaultSpeed * 2f : defaultSpeed;
        torqueForce = (Time.timeScale != 1) ? defaultRotate * 1.5f : defaultRotate;

    }

    public void SetIsInputEnabled(bool val)
    {
        IsInputEnabled = val;
    }

}
