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
    private bool SpacePressed;
    #endregion

    #region Player UI
    [Header("Player UI")]
    [SerializeField] private GameObject shieldPic, slowMoPic,Healthup, shotgunPic, minigunPic;   //Icons on canvas to indicate which powerup the player has 
    #endregion

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  triggers a powerup when a player presses space
    /// </summary>
    void Update()
    {
        if (!photonView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpacePressed = true;
        }

        if (powerUpType.Contains("SLOMO") && SpacePressed)
        {
            slow.Activate(true);
            powerUpType = "";
            displaySlowMo(false);
        }

        else if (powerUpType.Contains("SHIELD") && SpacePressed)
        {
            // only allow it to be active if it is turned off
            if (!(shield.activeSelf))
            {
                photonView.RPC("spawnShield", RpcTarget.AllBufferedViaServer);
                displayShield(false);
            }
        }

        else if (powerUpType.Contains("HealthUp") && (Input.GetKeyDown(KeyCode.Space)))
        {
            //add 10 to the health 
            this.GetComponent<PlayerManager>().AddHealth(50.0f);
            displayHealthUP(false);
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
        displayHealthUP(false);

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
            displayHealthUP(true);
        }
        else if (powerUpType == "AdditionalLife")
        {
            //working fine 
            this.GetComponent<PlayerManager>().AddLife();
        }
        else if (powerType == "Shotgun")
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Shooting>().SetShotgun();
            displayShotgun(true);
            StartCoroutine(DisableShootingPowerUp(shotgunPic));
        }
        else if (powerType == "Minigun")
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Shooting>().SetMinigun();
            displayMinigun(true);
            StartCoroutine(DisableShootingPowerUp(minigunPic));
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

    public void displayHealthUP(bool state)
    {
        Healthup.SetActive(state);
    }

    public void displayShotgun(bool state)
    {
        shotgunPic.SetActive(state);
    }

    public void displayMinigun(bool state)
    {
        minigunPic.SetActive(state);
    }


    public GameObject getShield()
    {
        return shield;
    }

    private IEnumerator DisableShootingPowerUp(GameObject Shootingpowerup)
    {
        yield return new WaitForSeconds(15f);
        Shootingpowerup.SetActive(false);
    }

    public void setSpacePressed(bool state) { SpacePressed = state; }
}
