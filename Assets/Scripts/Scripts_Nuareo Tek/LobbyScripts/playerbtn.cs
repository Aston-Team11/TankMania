using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerbtn : MonoBehaviour
{
    private GameObject canvas,loading;
   // private bool trigger = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        loading = GameObject.Find("LoadingComponent");
        this.gameObject.transform.SetParent(canvas.transform, false);
        canvas.GetComponent<Lobby>().ResetSetStartCount(false); //reset lobby countdown
        this.gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        InvokeRepeating("lobbyloaded", 12f, 100f);
    }

    private void lobbyloaded()
    {
        this.gameObject.SetActive(true);
    }
}
