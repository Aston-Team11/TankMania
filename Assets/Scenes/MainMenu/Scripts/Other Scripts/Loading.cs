using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad  K Rahman <br></br>
/// checks connection repeatedly and loads an appropriate error screen
/// </summary>
public class Loading : MonoBehaviourPunCallbacks
{
    private RectTransform rectComponent;                //a rectangle to rotate around the circle
    private float rotateSpeed = 200f;                   //the rotation speed of the laoding circle
    private int  Countrepeats = 0;                          //the number of repeats for the invokeRepeating function

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        InvokeRepeating("CheckConnectionStatus", 5f, 1f);
    }

    private void Update()
    {
        rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// changes player scene when connection is establised else shows an error message
    /// </summary>
    private void CheckConnectionStatus()
    {
        Countrepeats++;

        //if it has taken more than 54 seconds to connect then disconnect and show error message
        if (Countrepeats > 9)
        {
            PhotonNetwork.Disconnect();
            this.gameObject.SetActive(false);
        }

    }


}
