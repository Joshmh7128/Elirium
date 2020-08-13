using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public PlayerControlZ player;
    public bool isHolding; //check to see if we're holding something
    public Transform cameraPos;
	private Orb_PuzzleScript _heldOrb;

    [Range(0,15)]public float throwPower = 5;

    //setup our ray to detect anything aside from the player
    private LayerMask layerMask;
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
        layerMask = 1<<9; //get bitmap
        layerMask = ~layerMask; //invert the layermask

        dialogueManager = FindObjectOfType<DialogueManager>();
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
            else if (Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.forward), out hit, 4.5f, layerMask))
            {
                if (hit.collider.GetComponent<Orb_PuzzleScript>())
                {
                    hit.collider.gameObject.GetComponent<Orb_PuzzleScript>().isHeld = !hit.collider.gameObject.GetComponent<Orb_PuzzleScript>().isHeld;
                    isHolding = !isHolding;
                    _heldOrb = hit.collider.gameObject.GetComponent<Orb_PuzzleScript>();
                }
                else if (hit.collider.GetComponent<Interactable>())
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                    StartCoroutine(MoveCamera(0.35f, hit));
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
        else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) && !player.playerCanMoveInternal)
        {
            dialogueManager.DisplayNextSentence();
        }
    }

    private IEnumerator MoveCamera(float delay, RaycastHit slap)
    {
        yield return new WaitForSeconds(0.1f);

        while (!player.isGrounded)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(delay - 0.1f);

        tempVector = Vector3.Normalize(slap.collider.transform.GetChild(0).transform.position - transform.position);
        tempVectorHor = Vector3.Normalize(new Vector3(tempVector.x, 0, tempVector.z));
        tempForward = transform.TransformDirection(Vector3.forward);
        tempVectorVert = tempForward;
        tempVectorVert.y = tempVector.y;

        player.targetAngles.y += Vector3.SignedAngle(Vector3.Normalize(new Vector3(tempForward.x, 0, tempForward.z)), tempVectorHor, Vector3.up);
        player.targetAngles.x -= Vector3.SignedAngle(Vector3.Normalize(tempForward), tempVectorVert, transform.TransformDirection(Vector3.right));
    }
}
