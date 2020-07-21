using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformAttach : MonoBehaviour
{
    public GameObject Player;
	private List<GameObject> _children = new List<GameObject>();

    private void Awake()
    {

    }

    private void Start()
    {
        Player = GameObject.Find("PlayerZ");
    }

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player || other.GetComponent<Orb_PuzzleScript>())
        {
			if (other.transform.parent == null)
			{
				other.transform.parent = transform;
                _children.Add(other.gameObject);
			}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player || other.GetComponent<Orb_PuzzleScript>())
        {
            if (other.transform.parent == transform && other.gameObject == Player)
            {
                other.transform.parent = null;
                DontDestroyOnLoad(Player);
                _children.Remove(other.gameObject);
            }
            else if (other.transform.parent == transform)
                other.transform.parent = null;
            _children.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Detaches all children from this object
    /// </summary>
    public void Detach()
    {
        foreach (GameObject child in _children)
        {
            Debug.Log("Detach " + child.name);
            child.transform.parent = null;
            if (child == Player)
                DontDestroyOnLoad(Player);
        }

        _children = new List<GameObject>();
    }
}
