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

    public GameObject lastSentenceObject;
    public TextMeshProUGUI lastName;
    public TextMeshProUGUI lastSentence;

    #region States
    [HideInInspector] public bool _listening;
    [HideInInspector] public bool isTyping;
    [HideInInspector] public bool _answering;
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

        lastSentenceObject.SetActive(false);
        
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
            lastName.text = dialogue.name;
        }
        else
        {
            nameImage.enabled = false;
            nameText.enabled = false;
            lastName.text = dialogue.name;
        }

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        fadeSpeedMultiplier = dialogue.typeSpeed;

        DisplayNextSentence();
    }

    private string sentence;
    private string tempSentence;

    public void DisplayNextSentence()
    {
        if (!isTyping)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
        
            sentence = sentences.Dequeue();

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
        else
        {
            StopAllCoroutines();
            isTyping = false;

            bool tempBold = false;
            bool tempItalics = false;

            foreach (char character in sentence.ToCharArray())
            {
                if (Equals(character, emphasisCharacter))
                {
                    tempSentence += "";
                }
                else if (Equals(character, boldCharacter))
                {
                    if (!tempBold)
                    {
                        tempSentence += "<b>";
                    }
                    else {
                        tempSentence += "</b>";
                    }
                    tempBold = !tempBold;
                }
                else if (Equals(character, italicCharacter))
                {
                    if (!tempItalics)
                    {
                        tempSentence += "<i>";
                    }
                    else
                    {
                        tempSentence += "</i>";
                    }
                    tempItalics = !tempItalics;
                }
                else
                {
                    tempSentence += character;
                }
            }

            dialogueText.text = tempSentence;
            lastSentence.text = tempSentence;

            tempSentence = "";
        }
    }

    private string letter1;
    private string letter2;
    private float fadeSpeedMultiplier;
    private float colorFloat = 0;
    private int colorInt;
    private int letterCounter = 0;
    private string tempText;

    private string tempChar;

    [Tooltip("Character used turn bold text off or on./r/nWARNING: This character will not be displayed.")] public char boldCharacter;
    [Tooltip("Character used to turn italicized text on or off./r/nWARNING: This character will not be displayed.")] public char italicCharacter;
    [Tooltip("Character used to create a pause during dialogue./r/nWARNING: This character will not be displayed.")] public char emphasisCharacter;
    [Tooltip("How long the pause is during emphasis.")] public float emphasisTime = .5f;

    private bool boldOn;
    private bool italicsOn;

    IEnumerator TypeSentence(string sentence)
    {
        letterCounter = 0;
        tempText = "";
        boldOn = false;
        italicsOn = false;
        isTyping = true;
        
        while ((letterCounter < sentence.Length) && isTyping)
        {
            if (sentence[letterCounter] == emphasisCharacter)
            {
                letterCounter++;
                yield return new WaitForSeconds(emphasisTime);
            }
            else if (sentence[letterCounter] == boldCharacter)
            {
                boldOn = !boldOn;
                letterCounter++;
            }
            else if (sentence[letterCounter] == italicCharacter)
            {
                italicsOn = !italicsOn;
                letterCounter++;
            }
            else if (colorFloat < 1.0f)
            {
                colorFloat += Time.deltaTime * fadeSpeedMultiplier;
                colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colorFloat) * 255.0f);
                dialogueText.text = tempText;
                tempChar = sentence[letterCounter] + "";

                if (boldOn)
                {
                    tempChar = "<b>" + tempChar + "</b>";
                }
                if (italicsOn)
                {
                    tempChar = "<i>" + tempChar + "</i>";
                }

                tempChar = "<color=#FFFFFF" + string.Format("{0:X2}", colorInt) + ">" + tempChar + "</color>";

                dialogueText.text += tempChar;
            }
            else
            {
                colorFloat = 0.1f;
                tempText += tempChar;
                letterCounter++;
            }
            yield return null;
        }

        DisplayNextSentence();
    }

    private void EndDialogue()
    {
        _listening = false;

        lastSentenceObject.SetActive(false);

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

        lastSentenceObject.SetActive(true);
    }
}
