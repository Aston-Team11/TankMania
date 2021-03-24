using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// Handels rotating the top of the tank according to the mouse position 
/// </summary>
public class mouseTargetSwivel : MonoBehaviourPunCallbacks
{
    private Vector3 target;                             // the position of the mouse
    private GameObject reticle;                         //the mouse reticle in game,which aligns with the position of the mouse  
    public GameObject swivelTop;                        //the top of the tank including the barrel

    private RaycastHit hit;                             //a raycast used to determine the position of the mouse 

    public void SetMouseAim(GameObject Mousetarget)
    {
        reticle = Mousetarget;
    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// moves the top of the tank according to the position of the mouse in game 
    /// </summary>
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        // shoots ray from camera to mouse position;
        target = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(target);


        // checks if raycase hit something, then assign the postion to target vector
        if (Physics.Raycast(ray, out hit))
        {
            target = hit.point;
        }


        try
        {
            // visualise mouse cursor with target position
            reticle.GetComponent<Rigidbody>().MovePosition(target);

            // sviwel the tank's top to the mouse target position
            Vector3 difference = target - swivelTop.transform.position;
            float angleY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;

            // rotate slowly towards mouse Y postition
            Quaternion rotTarget = Quaternion.Euler(0.0f, angleY, 0.0f);
            swivelTop.transform.rotation = Quaternion.RotateTowards(swivelTop.transform.rotation, rotTarget, 20f);
        }

        catch (NullReferenceException)
        {
            return;
        }


    }
}


