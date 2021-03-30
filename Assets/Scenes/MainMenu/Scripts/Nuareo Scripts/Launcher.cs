using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;           // To be changed over
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private int Countrepeats = 0;
    private bool JoinState = false;             //joinstate defines how we should connect to the server (false we create a room nad true is to join a room)

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to server(Settings: APP ID, Region, Server Address) calls OnConnectedToMaster()
        PhotonNetwork.GameVersion = "1.0.0";
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnClickCreate() 
    {
        InvokeRepeating("CheckConnectionStatus", 3f, 1f);
        JoinState = false;
    }


     public void OnClickJoin(string roomName)
     {
        
        this.roomName = roomName;
        InvokeRepeating("CheckConnectionStatus", 3f, 1f);
        JoinState = true;

    }


    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// checks connection state and creates room when connected 
    /// </summary>
    private void CheckConnectionStatus()
    {
        Countrepeats++;
        
        if(Countrepeats > 100)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            bool state = false;

            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    state = true;
                }
               
            }

            if (state) { 
                if(JoinState == false)
                {
                    CreateRoom();       // tries to create a room
                    CancelInvoke();
                }
               else
                {
                    PhotonNetwork.JoinRoom(roomName);
                    CancelInvoke();
                }
            }           
        }
       
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
    /// Loads this player into the game 
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

    // To be caught, only called upon failure

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        string failure = "Connection Failed!\nPlease try again\n" + returnCode + ", " + message;
        GameObject failurebox = GameObject.FindGameObjectWithTag("ConnectFailure");
        failurebox.GetComponent<Text>().text = failure;
        failurebox.SetActive(true);
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);
        string failure = "Connection Failed!\nPlease try again\n" + returnCode + ", " + message;
        GameObject failurebox = GameObject.FindGameObjectWithTag("ConnectFailure");
        failurebox.GetComponent<Text>().text = failure;
       // failurebox.SetActive(true);
        
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        string failure = "Connection Failed!\nPlease try again\n" + errorInfo;
        GameObject failurebox = GameObject.FindGameObjectWithTag("ConnectFailure");
        failurebox.GetComponent<Text>().text = failure;
        failurebox.SetActive(true);
        base.OnErrorInfo(errorInfo);
    }
}


