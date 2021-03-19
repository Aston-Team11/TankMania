﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float piclength, startpos;
    public GameObject cam;
    public float paralllaxEffect;
    [SerializeField] bool offsetval;
    [SerializeField] private float offset;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        piclength = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void FixedUpdate()
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