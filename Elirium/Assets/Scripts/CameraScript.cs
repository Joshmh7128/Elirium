using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public PlayerControlZ player;
    public bool isHolding; //check to see if we're holding something
    public Transform cameraPos;
	private Orb_PuzzleScript _heldOrb;
    private GameObject holdPointPlayer;

    [Range(0,15)]public float throwPower = 5;

    //setup our ray to detect anything aside from the player
    public LayerMask layerMask;
    private RaycastHit hit;            //define our raycast

    #region Some Vectors
    Vector3 tempVector;
    Vector3 tempVectorHor;
    Vector3 tempForward;
    Vector3 tempVectorVert;
    #endregion

    DialogueManager dialogueManager;

    private void Awake()
    {
        //layerMask = 1<<9; //get bitmap
        //layerMask = ~layerMask; //invert the layermask

        dialogueManager = FindObjectOfType<DialogueManager>();
        holdPointPlayer = GameObject.Find("Hold Point Player");
    }

    //cast a spherecast forwards and see if we collide with an orb to pickup (or something else that can be interacted with)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && player.playerCanMoveInternal)
        {
            if (_heldOrb != null)
            {
                _heldOrb.isHeld = false;
                isHolding = false;
                _heldOrb = null;
            }
            else if (Physics.SphereCast(transform.position - transform.TransformDirection(Vector3.forward) / 2, 0.5f, transform.TransformDirection(Vector3.forward), out hit, 5f, layerMask))
            {
                if (hit.collider.GetComponent<Orb_PuzzleScript>())
                {
                    _heldOrb = hit.collider.gameObject.GetComponent<Orb_PuzzleScript>();
                    _heldOrb.isHeld = true;
                    isHolding = true;
                    _heldOrb.holdPoint = holdPointPlayer.transform;
                }
                else if (hit.collider.GetComponent<Interactable>())
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && _heldOrb != null)
        {
            _heldOrb.isHeld = false;
            _heldOrb.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * 500 * throwPower);
            isHolding = false;
            _heldOrb = null;
        }
        else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) &&  dialogueManager._listening)
        {
            dialogueManager.DisplayNextSentence();
        }
    }
}
