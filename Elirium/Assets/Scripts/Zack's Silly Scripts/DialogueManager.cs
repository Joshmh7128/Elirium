using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;

    private Queue<string> sentences;

    PlayerControlZ player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerControlZ>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        player.playerCanMoveInternal = false;

        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        
    }

    private string letter1;
    private string letter2;
    [SerializeField] private float fadeSpeedMultiplier = 4.5f;
    private float colorFloat = 0;
    private int colorInt;
    private int letterCounter = 0;
    private string tempText;

    private bool listening;

    IEnumerator TypeSentence(string sentence)
    {
        letterCounter = 0;
        tempText = "";
        
        while (letterCounter < sentence.Length)
        {
            if (colorFloat < 1.0f)
            {
                colorFloat += Time.deltaTime * fadeSpeedMultiplier;
                colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colorFloat) * 255.0f);
                dialogueText.text = tempText;
                dialogueText.text += "<color=#FFFFFF" + string.Format("{0:X2}", colorInt) + ">" + sentence[letterCounter] + "</color>";
            }
            else
            {
                colorFloat = 0.1f;
                tempText += sentence[letterCounter];
                letterCounter++;
            }
            yield return null;
        }
    }

    private void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        player.playerCanMoveInternal = true;
    }
}
