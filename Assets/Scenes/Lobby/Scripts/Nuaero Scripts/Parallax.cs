using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author Riyad K Rahman, Waheedullah Jan <br></br>
/// Handles parallax of background elements to give an infinite scroll effect 
/// </summary>
public class Parallax : MonoBehaviour
{
    private float piclength, startpos;                  //describes length in pixels of the sprite and the start postion of the gameobject.
    public GameObject cam;                              // the camera Gameobject is used to track the position of the camera
    public float paralllaxEffect;                       // the factor of parallax that should be applied 
    [SerializeField] bool offsetval;                    // to determine whether to apply an offset value or not 
    [SerializeField] private float offset;              // an offset to move pictures which have an offsetted sprite boundary

    /// <summary>
    /// @author Riyad K Rahman, Waheedullah Jan <br></br>
    /// intialises the start position and picture length variables
    /// </summary>
    private void Start()
    {
        startpos = transform.position.x;                            
        piclength = GetComponent<SpriteRenderer>().bounds.size.x;   

    }

    /// <summary>
    ///  @author Riyad K Rahman, Waheedullah Jan <br></br>
    ///  applies the parallax effect to the sprite and move the picture when the camera has scrolled past it  
    /// </summary>
    private void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - paralllaxEffect));
        float dist = (cam.transform.position.x * paralllaxEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (offsetval == false)
        {
            if (temp > startpos + piclength)
            {
                startpos += piclength;
            }
            else if (temp < startpos - piclength)
            {
                startpos -= piclength;
            }
        }

        else
        {
            if (temp > startpos + piclength + offset)
            {
                startpos += piclength + offset;
            }
            else if (temp < startpos - piclength + offset)
            {
                startpos -= piclength + offset;
            }
        }
    }
}
