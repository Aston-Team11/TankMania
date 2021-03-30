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
    [SerializeField] private GameObject connectionfail;  //text to inform user the connection has failed 

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
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
        if (Countrepeats > 2)
        {
            PhotonNetwork.Disconnect();
            transform.parent.gameObject.SetActive(false);
            CancelInvoke();
        }

    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// shows error text when this object is disabled 
    /// </summary>
    public override void OnDisable()
    {
        connectionfail.SetActive(true);
        CancelInvoke();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// begins a countdown to disconnect
    /// </summary>
    public override void OnEnable()
    {
        connectionfail.SetActive(false);
        Countrepeats = 0;
        InvokeRepeating("CheckConnectionStatus", 5f, 1f);
    }

}
