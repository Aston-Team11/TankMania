using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerbtn : MonoBehaviour
{
    private GameObject canvas;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        this.gameObject.transform.SetParent(canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
