using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeepoExploresTest : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform ball;
    public Transform player;

    private Collider _heldCollider;
    private Orb_PuzzleScript _heldOrb;

    private GameObject holdPointPeepo;

    private Vector3 origin;

    private bool isHolding = false;

    private void Start()
    {
        holdPointPeepo = GameObject.Find("Hold Point Peepo");
        origin = transform.position;
    }

    void Update()
    {
        if (!isHolding)
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(ball.position);
        }
        else
        {
            agent.stoppingDistance = 5;
            agent.SetDestination(player.position);
        }

        if (_heldOrb != null && !isHolding)
        {
            if (!_heldOrb.isHeld)
            {
                _heldOrb.holdPoint = holdPointPeepo.transform;
                _heldOrb.isHeld = true;
                isHolding = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Orb_PuzzleScript>())
        {
            _heldCollider = other;
            _heldOrb = other.GetComponent<Orb_PuzzleScript>();

            if (!_heldOrb.isHeld)
            {
                _heldOrb.holdPoint = holdPointPeepo.transform;
                _heldOrb.isHeld = true;
                isHolding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Collider.ReferenceEquals(other, _heldCollider))
        {
            isHolding = false;
            _heldCollider = null;
            _heldOrb = null;
        }
    }
}
