using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// @author Waheedullah Jan <br></br>
/// handles when to change scenes
/// </summary>
public class Logo : MonoBehaviour
{
   [SerializeField] private float wait_time = 7f;   //the wait time until we laod a new scene


    /// <summary>
    /// @author Waheedullah Jan <br></br>
    /// begins the Coroutine <see cref="wait_for_logo"/> 
    /// </summary>
    void Start()
    {
      StartCoroutine(wait_for_logo());  
    }

    /// <summary>
    /// @author Waheedullah Jan <br></br>
    /// waits 7 seconds then loads scene 1 
    /// </summary>
    IEnumerator wait_for_logo()

    {
      yield return new WaitForSeconds(wait_time);
      SceneManager.LoadScene(1);
    }
}
