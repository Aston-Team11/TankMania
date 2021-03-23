﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPowerupManager : MonoBehaviourPunCallbacks
{
    #region Powerups
    [Header("Powerups")]
    [SerializeField] private GameObject shield;         //shield powerup
    private Slo_Motion slow;                            //slomotion powerup
    private bool slo;                                   //used to enable/disable slo motion
    private string powerUpType = "";                    //stores the current powerup as a string
    #endregion

    #region Player UI
    [Header("Player UI")]
    [SerializeField] private GameObject shieldPic, slowMoPic;   //Icons on canvas to indicate which powerup the player has 
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if (powerUpType.Contains("SLOMO") && (Input.GetKeyDown(KeyCode.Space)))
        {
            slo = !slo;
            slow.Activate(slo);
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

    [PunRPC]
    public void spawnShield()
    {
        shield.SetActive(true);
        shield.SendMessage("resetFernal", -0.2f);
        powerUpType = "";
    }


    public void PowerupAttained(string powerType)
    {
        displayShield(false);
        displaySlowMo(false);
        powerUpType = powerType;
        if (powerUpType == "SHIELD")
        {
            displayShield(true);
        }
        else if (powerUpType == "SLOMO")
        {
            displaySlowMo(true);
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