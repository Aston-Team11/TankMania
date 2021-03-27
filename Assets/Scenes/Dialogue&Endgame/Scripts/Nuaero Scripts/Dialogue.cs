using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// The dialogue class will be used as an object that will be passed into the dialogue manager whenever a dialogue is started 
// This class also host all information that is need for the dialogue.

public class Dialogue
{
    [TextArea(3, 15)] // The first variable is the minimum amount of lines the text area area will use and the second varibale is the maximum lines in the text area. 
    public string[] sentences; // Sentencess that will be loaded into the queue.
}
