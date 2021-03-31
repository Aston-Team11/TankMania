using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles displaying game menus depending on player input and managing the data for each menu canvas
/// </summary>
public class InGameMenusManager : MonoBehaviourPunCallbacks
{
    private static bool GameIsPaused = false;
    private int gameMode;
    [SerializeField] private GameObject pauseMenuUI, leaderboardUI;
    public int[] sharedPlayerstats; 

    private void Start()
    {
        var launcher = GameObject.Find("Launcher").GetComponent<Launcher>();
        gameMode = launcher.GetGameMode();
        
        //set default value for the player lives stats in the pve gamemode 
        if (gameMode == 0)
        {
            sharedPlayerstats[0] = 10;
            sharedPlayerstats[1] = 10;
            sharedPlayerstats[2] = 10;
            sharedPlayerstats[3] = 10;
        }
    }

    /// <summary>
    /// @author Balraj Bains,Riyad K Rahman <br></br>
    /// shows an appropraite menu screen depending on the input and the current gamemode 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))

        {
            if (GameIsPaused)

            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            leaderboardUI.SetActive(true);
            GameObject GamemodeCanvas = leaderboardUI.transform.GetChild(gameMode).gameObject;
            GamemodeCanvas.SetActive(true);

            //only display player stats for the number of players in the game 
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                GamemodeCanvas.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderboardUI.SetActive(false);
        }
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// hides pause menu screen
    /// </summary>
    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// shows pause menu screen
    /// </summary>
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// takes user back to main menu screen and disconnects player from the network
    /// </summary>
    public void LoadMenu()
    {
        Destroy(GameObject.Find("Launcher"));
        SceneManager.LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// @author Balraj Bains <br></br>
    /// Exits the entire application 
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Add a kill for the player who matches the player id
    /// </summary>
    public void AddKills(int playerID , int killcount)
    {
        photonView.RPC("SyncKillCount", RpcTarget.AllBuffered, playerID, killcount);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Sync kills for all players 
    /// </summary>
    [PunRPC]
    private void SyncKillCount(int playerID, int killcount)
    {
        sharedPlayerstats[playerID] = killcount;
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Set lives for the player who matches the player id
    /// </summary>
    public void ChangeLives(int playerID, int lives)
    {
        photonView.RPC("SyncLives", RpcTarget.AllBuffered, playerID, lives);
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Sync lives for all players 
    /// </summary>
    [PunRPC]
    private void SyncLives(int playerID, int lives)
    {
        sharedPlayerstats[playerID] = lives;
    }

    public int GetPlayerStats(int playerID)
    {
        return sharedPlayerstats[playerID];
    }
}
