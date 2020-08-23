using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Dialogue.FunctionCalls[] functions;
    private DialogueManagerAux dmAux;

    public Image nameImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;

    private Queue<string> sentences;

    PlayerControlZ player;

    private IEnumerator coroutine;

    #region States
    public bool _listening;
    //public bool _waiting;
    public bool _answering;
    #endregion

    private void Awake()
    {
        player = FindObjectOfType<PlayerControlZ>();
        dmAux = FindObjectOfType<DialogueManagerAux>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _listening = true;
        _answering = false;

        ButtonScrollList.SetActive(false);
        dialogueText.enabled = true;

        dmAux.DoThing(0.35f, dialogue.lookPoint);

        functions = dialogue.Functions;

        player.playerCanMoveInternal = false;
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;

        animator.SetBool("IsOpen", true);

        if (dialogue.name != "")
        {
            nameImage.enabled = true;
            nameText.enabled = true;
            nameText.text = dialogue.name;
        }
        else
        {
            nameImage.enabled = false;
            nameText.enabled = false;
        }

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        fadeSpeedMultiplier = dialogue.typeSpeed;

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

        if (sentence != "")
        {
            animator.SetBool("IsOpen", true);
            dialogueText.enabled = true;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }
        else
        {
            animator.SetBool("IsOpen", false);
            dialogueText.enabled = false;
        }
    }

    private string letter1;
    private string letter2;
    private float fadeSpeedMultiplier;
    private float colorFloat = 0;
    private int colorInt;
    private int letterCounter = 0;
    private string tempText;

    // private bool listening;

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
        _listening = false;

        if (functions.Length == 0)
        {
            animator.SetBool("IsOpen", false);
            player.playerCanMoveInternal = true;
            Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        }
        else
        {
            foreach (Dialogue.FunctionCalls thing in functions)
            {
                if (thing.gameObject != null && thing.methodName != "")
                {
                    thing.gameObject.SendMessage(thing.methodName);
                }
            }
        }
    }

    public GameObject ButtonScrollList;
    public ButtonListControl listController;

    public void SetButtons(Choice[] choices)
    {
        ButtonScrollList.SetActive(true);
        nameImage.enabled = false;
        nameText.enabled = false;
        dialogueText.enabled = false;
        _listening = false;
        _answering = true;

        listController.GenerateList(choices);
    }
}
