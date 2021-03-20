using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerbtn : MonoBehaviour
{
    [SerializeField] private GameObject canvas,loading,errorscreen;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        loading = GameObject.Find("LoadingComponent");
        errorscreen = GameObject.Find("ErrorScreen");
        this.gameObject.transform.SetParent(canvas.transform, false);
        canvas.GetComponent<Lobby>().ResetSetStartCount(false); //reset lobby countdown
        this.gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        InvokeRepeating("lobbyloaded", 12.2f, 100f);
    }

    private void lobbyloaded()
    {
        if (errorscreen == null)
        {
            this.gameObject.SetActive(true);
        }
             
       CancelInvoke();
    }
}
