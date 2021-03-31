using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class InGameMenus : MonoBehaviourPunCallbacks
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

    // Update is called once per frame
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

    public void Resume()

    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()

    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Destroy(GameObject.Find("Launcher"));
        SceneManager.LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AddKills(int playerID , int killcount)
    {
        photonView.RPC("SyncKillCount", RpcTarget.AllBuffered, playerID, killcount);
    }

    [PunRPC]
    private void SyncKillCount(int playerID, int killcount)
    {
        sharedPlayerstats[playerID] = killcount;
    }


    public void ChangeLives(int playerID, int lives)
    {
        photonView.RPC("SyncLives", RpcTarget.AllBuffered, playerID, lives);
    }

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
