using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// Handles camera movement in dialog,menu,lobby,endgame scenes
/// </summary>
public class LobbyCameraMovement : MonoBehaviour
{
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Scrolls camera infinitely to the right
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 campos = transform.position;
        campos.x += 10 * Time.fixedDeltaTime;
        transform.position = campos;
    }
}
