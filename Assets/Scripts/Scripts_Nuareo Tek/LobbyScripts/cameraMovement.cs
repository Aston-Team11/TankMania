using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    // Scrolls camera infinitely to the right
    void FixedUpdate()
    {
        Vector3 campos = transform.position;
        campos.x += 10 * Time.fixedDeltaTime;
        transform.position = campos;
    }
}
