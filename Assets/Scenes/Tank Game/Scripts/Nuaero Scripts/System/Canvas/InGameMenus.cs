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
    public int[] playerKills; 

    private void Start()
    {
        var launcher = GameObject.Find("Launcher").GetComponent<Launcher>();
        gameMode = launcher.GetGameMode();
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
            leaderboardUI.transform.GetChild(gameMode).gameObject.SetActive(true);
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

    public int GetPlayerKills(int playerID)
    {
        return playerKills[playerID];
    }

    public void AddKills(int playerID , int killcount)
    {
        photonView.RPC("SyncKillCount", RpcTarget.AllBuffered, playerID, killcount);
    }

    [PunRPC]
    private void SyncKillCount(int playerID, int killcount)
    {
        playerKills[playerID] = killcount;
    }

}
