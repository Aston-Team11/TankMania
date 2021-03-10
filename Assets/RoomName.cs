﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomName : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
        this.gameObject.GetComponent<Text>().text = "Room Name: " + GameObject.Find("Launcher").GetComponent<Launcher>().getRoomName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}