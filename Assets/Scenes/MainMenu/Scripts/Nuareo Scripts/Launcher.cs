﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;           // To be changed over
using UnityEngine.SceneManagement;


/// <summary>
/// @author Balraj Bains, Riyad K Rahman<br></br>
/// Handles the connection to a server and creating/joining a room.
/// </summary>
public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]private string roomName;                      //the 5 letter roomname which is to be randomly generated 
    static private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";   // characters to choose from to generate a room name
    [SerializeField] private int gameMode;                        // the gamemode selected 
    private bool testing = false;


    private void Awake()
    {
        StartCoroutine(AwakeDelay());
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnClickCreate() 
    {
        PhotonNetwork.ConnectUsingSettings();   // Connect to server (Settings: APP ID, Region, Server Address) calls OnConnectedToMaster()
        StartCoroutine(roomCreate());           // Call roomCreate()
    }


     public void OnClickJoin(string roomName)
     {
        PhotonNetwork.ConnectUsingSettings();
        this.roomName = roomName;
        StartCoroutine(JoinRoom());
         
     }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// Waits for three second before calling CreateRoom()
    /// </summary>
    public IEnumerator roomCreate()
    {
        yield return new WaitForSeconds(3); 
        CreateRoom();
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// Waits for three seconds before joining a room
    /// </summary>
    public IEnumerator JoinRoom()
    {
        yield return new WaitForSeconds(3); //
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Waits for three seconds before auhthenciating room version
    /// </summary>
    public IEnumerator AwakeDelay()
    {
        yield return new WaitForSeconds(3); // Waits
        //PhotonNetwork.AutomaticallySyncScene = true; // Sets options to always sync the scene to who is the Master client (Host)
        PhotonNetwork.GameVersion = "1.0.0";
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Loads Scene 3 
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (testing == false)
        {
            PhotonNetwork.LoadLevel(3);
            base.OnJoinedRoom();
        }
        else
        {
            PhotonNetwork.LoadLevel(4);
            base.OnJoinedRoom();
        }
    }


    /// <summary>
    /// @author Balraj Bains <br></br>
    /// Creates a room with a randomly generated room name
    /// </summary>
    public void CreateRoom()
    {
        roomName = "";
        for (int i = 0; i < 5; i++) //Create a roomName of 5 chars
        {
            roomName += chars[Random.Range(0, chars.Length)];
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; // Set max players in a room to be 4
        PhotonNetwork.CreateRoom(roomName, roomOptions); // Create a room with Name (str) and roomOptions
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// returns room name
    /// </summary>
    public string GetRoomName() { return roomName; }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// sets the gamemode 
    /// </summary>
    public void SetGameMode(int mode) {
        this.gameMode = mode;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// </summary>
    /// <returns> returns the gamemode </returns>
    public int GetGameMode()
    {
        return this.gameMode; 
    }


    public void SetTesting(bool state)
    {
        testing = state;
    }
}


