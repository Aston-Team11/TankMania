using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Wait : MonoBehaviour
{
    [SerializeField] private float wait_time = 12f;
    [SerializeField] private GameObject bg, lobby,errorscreen, mySession;
    private bool showerror = false;

    void Start()
    {
        StartCoroutine(checkJoin());
    }

    /// <summary>
    /// if a new player tries to join in-game session they are disconnected
    /// </summary>
    IEnumerator checkJoin()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Wait_for_intro());
    }



        /// <summary>
        /// Does something after the loading screen 
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
