using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  @author Christian Corfield <br></br>
///  gets the appropriate data for the roundnumber and displays this on the player's UI
/// </summary>
public class HealthBar : MonoBehaviour
{

    public Slider slider;                                   //the slider component to illustrate the amount of health remaning 
    public Gradient gradient;                               //a gradient to change colors depending on the amount of health 
    public Image fill;                                      //image component attached to the healthbar

    [SerializeField] private PlayerManager playerstats;     //the player's stats (who owns this healthbar)

    /// <summary>
    ///  @author Christian Corfield <br></br>
    ///  intialses the max and current health values 
    /// </summary>
    public void Start()
    {
        SetMaxHealth(playerstats.GetHealth());
        SetHealth(playerstats.GetHealth());
    }

    /// <summary>
    ///  @author Christian Corfield <br></br>
    ///  updates the health component in the UI every frame with the player's current health 
    /// </summary>
    private void Update()
    {
        playerstats.GetHealth();
    }


    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
