using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Wait : MonoBehaviour
{
    [SerializeField] private float wait_time = 12f;
    [SerializeField] private GameObject bg, lobby;

    void Start()
    {
      StartCoroutine(Wait_for_intro());
    }

    IEnumerator Wait_for_intro()

    {
      yield return new WaitForSeconds(wait_time);
       //enable lobby screen
        bg.SetActive(true);
        lobby.SetActive(true);
    }



}
