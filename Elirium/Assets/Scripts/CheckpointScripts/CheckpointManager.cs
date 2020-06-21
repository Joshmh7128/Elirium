using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Vector3 playerPosition;
    public List<Vector3> orbPositions = new List<Vector3>();

    public PlayerController player;
    public static List<Orb_PuzzleScript> Orbs { get; set; } = new List<Orb_PuzzleScript>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(player.gameObject);

        UpdateOrbs();

        playerPosition = player.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckpointLoad();
        }
    }

    public void CheckpointLoad()
    {
        player.ResetPos(playerPosition);

        for (int i = 0; i < Orbs.Count; i++)
        {
            if (Orbs[i] != null)
            {
                Orbs[i].ResetPos(orbPositions[i]);
            }
            
        }
    }

    public void UpdateOrbs()
    {
        Orbs = new List<Orb_PuzzleScript>();
        orbPositions = new List<Vector3>();
        Orb_PuzzleScript[] orbArray = FindObjectsOfType<Orb_PuzzleScript>();
        Debug.Log(orbArray.Length);
        for (int i = 0; i < orbArray.Length; i++)
        {
            Orbs.Add(orbArray[i]);
            orbPositions.Add(Orbs[i].transform.position);
        }
    }

    public void UpdatePlayerPos(Vector3 pos)
    {
        playerPosition = pos;
    }

    public void UpdateOrbPos(Vector3 pos, Orb_PuzzleScript orb)
    {
        if (Orbs.Contains(orb))
        {
            orbPositions[Orbs.IndexOf(orb)] = pos;
        }
    }

    public void SavePlayer()
    {
        DontDestroyOnLoad(player.gameObject);
    }
}
