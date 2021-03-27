using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText; // references to UI an where the story line (sentences) will be displayed in the UI
    public Animator animator; //Animator is used to hide and dispaly the dialogue.
    private Queue<string> sentences; // All the sentences that will be dispalyed in the dialogue will be held in this queue and as the user reads through the dialog new sentences will be loaded from the end of the queue. 
    public AudioSource continue_Sound; //sound for the audio
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();  // Initialize the queue
        continue_Sound = GetComponent<AudioSource>();
    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true); // when the start button is clicked a new dialgoue is started and displayed on the scene.

        sentences.Clear(); // This clears any pervious sentences in the dialogue.

        foreach (string sentence in dialogue.sentences) //Loops through all the sentences in the array.
        {
            sentences.Enqueue(sentence); // This will queue up a sentence and display the current sentence
        }
        DisplayNextSentence(); //This will dispaly the next sentence. The function will also be attached to the continue button.
    }

    // This method will be called from the continue button.
    // It will first check if there is any more sentencein the queue by using the if statement and if it is 0 it will end the dialogue.
    // However if there is more sentneces left in the queue, we will get the next sentnece in the queue by using the sentences.Dequeue() and store it in string sentence varibale.
    public void DisplayNextSentence()
    {
        continue_Sound.Play();
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // This make sure that if type sentence is already running it will stop doing so and start a new one.
        StartCoroutine(TypeSentence(sentence)); // This will start the TypeSentence co-routine.
    }

    // This method is used to display all the characters inisde the sentence one by one.
    // It will loop through all the individual characters in the sentence to do that we use a foreach loop and a ToCharArray function to convert a string into a character array.
    // After each letter it will take 0.02f to display the next one.
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            yield return new WaitForSeconds(0.02f);
            dialogueText.text += letter;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false); // when the last sentence is displayed and the user clicks the contuine button it close the dialogue scene and move on to the main menu scene. 
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); // It will load the main menu scene once the dialogue scene closes. 
    }
}
