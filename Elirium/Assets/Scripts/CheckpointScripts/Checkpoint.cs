using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public CheckpointManager manager;

    private bool isActive;

    public Dictionary<Vector3, Orb_PuzzleScript> storedOrbs = new Dictionary<Vector3, Orb_PuzzleScript>();

    private void Awake()
    {
        manager = FindObjectOfType<CheckpointManager>();
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            manager.UpdatePlayerPos(other.transform.position);
            foreach (Checkpoint point in transform.parent.GetComponentsInChildren<Checkpoint>())
            {
                point.setActive(false);
            }

            setActive(true);
            ClearStoredOrbs();
        }
        else if (isActive && other.GetComponent<Orb_PuzzleScript>())
        {
            manager.UpdateOrbPos(other.transform.position, other.GetComponent<Orb_PuzzleScript>());
        }
        else if (other.GetComponent<Orb_PuzzleScript>())
        {
            storedOrbs.Add(other.transform.position, other.GetComponent<Orb_PuzzleScript>());
        }
    }

    private void ClearStoredOrbs()
    {
        foreach (KeyValuePair<Vector3, Orb_PuzzleScript> orbData in storedOrbs)
        {
            manager.UpdateOrbPos(orbData.Key, orbData.Value);
        }

        storedOrbs = new Dictionary<Vector3, Orb_PuzzleScript>();
    }

    public void setActive(bool active)
    {
        isActive = active;
    }
}
