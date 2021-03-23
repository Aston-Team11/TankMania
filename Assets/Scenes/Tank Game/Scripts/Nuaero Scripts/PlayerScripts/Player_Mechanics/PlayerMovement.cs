﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private Rigidbody rb;

    public float acceleration, torqueForce;
    private float defaultRotate, defaultSpeed;
    private bool IsInputEnabled = true;

    public float steering;
    public Vector2 direction;
  
    public GameObject parentCam;
    [SerializeField] private AudioSource engine;
    [SerializeField] private float audioOffset; 
 

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




    // Update is called once per frame
    private void Update()
    {
        // check if player view is this player
        if (!photonView.IsMine) return;
        if(!IsInputEnabled) return;
        direction.x = Input.GetAxisRaw("Vertical");
        direction.y = Input.GetAxisRaw("Horizontal") * steering;
      
    }


    //handles physics
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