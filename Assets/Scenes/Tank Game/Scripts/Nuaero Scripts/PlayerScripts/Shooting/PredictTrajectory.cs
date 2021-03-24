using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles Calculations of the trajectory of the bullet
/// </summary>
public class PredictTrajectory : MonoBehaviourPunCallbacks
{

    private LineRenderer line;                              // the line component used to illustrate the trajectory
    private Ray ray;                                        //the ray which is shot from the barrel to be used in trajectory calculations
    private RaycastHit hit;                                 // the Ray's hit poistion infomation 
    private int rayBounce;

    public int reflections;                                 //the max number of reflections to calculate the aim for 
    public float maxLength;                                 // the max length of the line 

    private GameObject mouseReticle;                              //the mouse reticle to calcualte the distance of the line with respect to the current mouse position 
   

    public void SetMouseAim(GameObject Mousetarget)
    {
        mouseReticle = Mousetarget;
    }




    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// calcualtes aim to a max number of <see cref="reflections"/> by shooting a ray and reflecting the ray based on collisions with reflective surfaces 
    /// </summary>
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        //shoots ray forward
        ray = new Ray(transform.position, transform.forward);
        rayBounce = 0;
        //intialise line to one direction (position count is the turning points of the line)
        line.positionCount = 1;
        line.SetPosition(0, transform.position);
        float remainingLength = maxLength;

        for(int i = 0; i< reflections; i++)
        {

            // if a ray hits something
                if (Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                
                    rayBounce++;
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, hit.point);
                    remainingLength -= Vector2.Distance(ray.origin, hit.point);

                // creates new ray from the hitpoint to relfected vector3
                    ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                   
               // if no "reflective surfaces hit then cancel
                if (hit.collider.tag != "reflectSurface")
                break;
            }

            else
            {
                try
                {
                    float distance = (rayBounce >= 1) ? remainingLength : Vector3.Distance(transform.position, mouseReticle.transform.position);
               

                // if no surface at all hit, then carry on line 
                line.positionCount += 1;
                line.SetPosition(line.positionCount - 1, ray.origin + ray.direction * distance);
                }

                catch (NullReferenceException)
                {
                    return;
                }
            }
        }
    }

}
