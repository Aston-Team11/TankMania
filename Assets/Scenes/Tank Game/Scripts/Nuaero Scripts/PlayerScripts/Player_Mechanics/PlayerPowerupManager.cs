using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
///  @author Riyad K Rahman <br></br>
///  manages use of powerups and UI components related to powerups 
/// </summary>
public class PlayerPowerupManager : MonoBehaviourPunCallbacks
{
    #region Powerups
    [Header("Powerups")]
    [SerializeField] private GameObject shield;         //shield powerup
    private Slo_Motion slow;                            //slomotion powerup
    private string powerUpType = "";                    //stores the current powerup as a string
    #endregion

    #region Player UI
    [Header("Player UI")]
    [SerializeField] private GameObject shieldPic, slowMoPic;   //Icons on canvas to indicate which powerup the player has 
    #endregion

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  triggers a powerup when a player presses space
    /// </summary>
    void Update()
    {
        if (!photonView.IsMine) return;

        if (powerUpType.Contains("SLOMO") && (Input.GetKeyDown(KeyCode.Space)))
        {
            slow.Activate(true);
            powerUpType = "";
            displaySlowMo(false);
        }

        else if (powerUpType.Contains("SHIELD") && (Input.GetKeyDown(KeyCode.Space)))
        {
            // only allow it to be active if it is turned off
            if (!(shield.activeSelf))
            {
                photonView.RPC("spawnShield", RpcTarget.AllBufferedViaServer);
                displayShield(false);
            }
        }
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  spawns a shield across all clients 
    /// </summary>
    [PunRPC]
    public void spawnShield()
    {
        shield.SetActive(true);
        shield.SendMessage("resetFernal", -0.2f);
        powerUpType = "";
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  sets the appropriate UI element to active depending on which powerup was attanied  
    /// </summary>
    /// <param name="powerType">the name of the powerup</param>
    public void PowerupAttained(string powerType)
    {
        powerUpType = powerType;

        if (!photonView.IsMine) return;

        displayShield(false);
        displaySlowMo(false);

        if (powerUpType == "SHIELD")
        {
            displayShield(true);
        }
        else if (powerUpType == "SLOMO")
        {
            displaySlowMo(true);
        }
        else if (powerUpType == "HealthUp")
        {
            //add 10 to the health 
            this.GetComponent<PlayerManager>().AddHealth(10.0f);

            //issue with not syncing with the health bar 
        }
        else if (powerUpType == "AdditionalLife")
        {
            //working fine 
            this.GetComponent<PlayerManager>().AddLife();
        }
        else if (powerType == "Shotgun")
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Shooting>().SetShotgun();

        }
        else if (powerType == "Minigun")
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Shooting>().SetMinigun();
        }
    }


    public void setTimeObject(GameObject time)
    {
        slow = time.GetComponent<Slo_Motion>();
    }

    public void displayShield(bool state)
    {
        shieldPic.SetActive(state);
    }

    public void displaySlowMo(bool state)
    {
        slowMoPic.SetActive(state);
    }

    public GameObject getShield()
    {
        return shield;
    }

}
