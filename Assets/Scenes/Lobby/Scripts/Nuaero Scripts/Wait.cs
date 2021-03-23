using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// @author Waheedullah Jan <br></br>
/// Handles assignment of text in the gameobject
/// </summary>
public class Wait : MonoBehaviour
{
    [SerializeField] private float wait_time = 12f;
    [SerializeField] private GameObject bg, lobby,errorscreen, mySession;
    private bool showerror = false;

    private void Start()
    {
        StartCoroutine(checkJoin());
    }

    /// <summary>
    /// @author Waheedullah Jan <br></br>
    /// if a new player tries to join in-game session they are disconnected
    /// </summary>
    IEnumerator checkJoin()
    {
        yield return new WaitForSeconds(0.1f);

        //check if a game session has already begun
        if (GameObject.FindGameObjectWithTag("GameSession") != null)
        {
            PhotonNetwork.Disconnect();
            showerror = true;
        }

        StartCoroutine(Wait_for_intro());
    }



    /// <summary>
    /// @author Waheedullah Jan <br></br>
    /// Enables a canvas depending on whether a GameSession was found or not 
    /// </summary>
    IEnumerator Wait_for_intro()
    {
      yield return new WaitForSeconds(wait_time);
        
        //enable lobby screen
        if (!showerror)
        {
            bg.SetActive(true);
            lobby.SetActive(true);
            this.gameObject.SetActive(false);
        }

        //otherwise show that game is already in session
        else
        {
            errorscreen.SetActive(true);
           this.gameObject.SetActive(false);
        }
        
    }


}
