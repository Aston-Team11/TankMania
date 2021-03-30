using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField] private Text timeText;
    private int seconds = 0;
    private float TotalTime = 0;


    /// <summary>
    /// updates the timer in the UI
    /// </summary>
    void Update()
    {
        TotalTime += Time.deltaTime;

        if(timeText.gameObject.activeSelf == true)
        {
            float minutes = Mathf.FloorToInt(TotalTime / 60);
            float seconds = Mathf.FloorToInt(TotalTime % 60);

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }
}
