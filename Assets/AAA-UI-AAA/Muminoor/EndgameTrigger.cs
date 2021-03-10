using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgameTrigger : MonoBehaviour
{
    public AudioSource start_Sound;
    public Dialogue dialogue;
    public void Start()
    {
        start_Sound = GetComponent<AudioSource>();
        start_Sound.Play();
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void TriggerDialogue()
    {
        start_Sound.Play();
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

}
