using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombiesRemaining : MonoBehaviour
{
    private Text zombiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        zombiesRemaining = GetComponent<Text>() as Text;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
        zombiesRemaining.text = list.Length.ToString();
    }
}

