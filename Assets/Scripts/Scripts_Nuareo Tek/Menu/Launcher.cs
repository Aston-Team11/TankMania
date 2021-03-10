using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;// To be changed over
using UnityEngine.SceneManagement;


/// <summary>
/// Basic script handling the connection to a server and creating and joining room.
/// </summary>
public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]private string roomName;
    static private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public void Awake()
    {
        StartCoroutine(AwakeDelay());
    }

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnClickCreate() 
    {
        PhotonNetwork.ConnectUsingSettings();  // Connect to server (Settings: APP ID, Region, Server Address) calls OnConnectedToMaster()
        StartCoroutine(roomCreate()); // Call roomCreate()
    }


     public void OnClickJoin(string roomName)
     {
        PhotonNetwork.ConnectUsingSettings();
        this.roomName = roomName;
        StartCoroutine(JoinRoom());
         
     }

    public IEnumerator roomCreate()
    {
        yield return new WaitForSeconds(2); // Waits for one second before calling CreateRoom()
        CreateRoom();
    }

    public IEnumerator JoinRoom()
    {
        yield return new WaitForSeconds(2); // Waits for one second before calling CreateRoom()
        PhotonNetwork.JoinRoom(roomName);
    }
    public IEnumerator AwakeDelay()
    {
        yield return new WaitForSeconds(2); // Waits
        PhotonNetwork.AutomaticallySyncScene = true; // Sets options to always sync the scene to who is the Master client (Host)
        PhotonNetwork.GameVersion = "1.5.4";
    }


    public override void OnJoinedRoom()
    {
        //StartGame();
        PhotonNetwork.LoadLevel(3);
        base.OnJoinedRoom();
        //SceneManager.LoadScene("Tank Game");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //PhotonNetwork.LoadLevel(3);
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1) // if there is one player in the room, load the level
        {
            PhotonNetwork.LoadLevel(3);
        }
    }

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

    public string getRoomName() { return roomName; }
   // public void setRoomName(string name) { roomName = name; }


}


