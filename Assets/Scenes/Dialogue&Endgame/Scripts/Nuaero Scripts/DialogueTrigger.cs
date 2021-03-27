using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This scrit sits on the startGmae Button and allows the user to trigger the dialogue.

public class DialogueTrigger : MonoBehaviour
{   // for the startGamebUtton.
    public AudioSource start_Sound;             // clicking sound when clicked on the start button.
    public void Start()
    {
        start_Sound = GetComponent<AudioSource>();
    }
    public Dialogue dialogue;

    // This will start the dialogue and display the sentences.
    public void TriggerDialogue()
    {
        start_Sound.Play();         //This will play the sound when clicked on the start button.
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue); // This will find the DialogueManager object and start the Dialogue.
    }
}
