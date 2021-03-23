using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles when to activate this gameobject
/// </summary>
public class playerbtn : MonoBehaviour
{
    [SerializeField] private GameObject canvas,loading,errorscreen;     //three canvas gameobjects. we need to know which is currently active to understand if we should activate the gameobject


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// if a game session has not already begun, then we place the player button into the canvas 
    /// </summary>
    private void Start()
    {

        if (GameObject.FindGameObjectWithTag("GameSession") != null)
        {
            this.gameObject.SetActive(false);
  
        }

        else
        {
            canvas = GameObject.Find("Canvas");
            loading = GameObject.Find("LoadingComponent");
            errorscreen = GameObject.Find("ErrorScreen");
            this.gameObject.transform.SetParent(canvas.transform, false);
            canvas.GetComponent<Lobby>().ResetSetStartCount(false); //reset lobby countdown
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// after 12.2 seconds calls the <see cref="lobbyloaded"/> function
    /// </summary>
    private void OnDisable()
    {
        InvokeRepeating("Lobbyloaded", 12.2f, 100f);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// if the errorscreen is not activated
    /// </summary>
    private void Lobbyloaded()
    {
        if (errorscreen == null)
        {
            this.gameObject.SetActive(true);
        }
             
       CancelInvoke();
    }
}
