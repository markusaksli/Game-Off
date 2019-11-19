using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public PlayerController playerDeathManager;

    public Renderer theRend;

    public Material cpOff;
    public Material cpOn;


    private void Start()
    {
        playerDeathManager = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        
    }
    public void CheckpointOn()
    {
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint cp in checkpoints)
        {
            cp.CheckpointOff();
        }

        theRend.material = cpOn;
    }
    public void CheckpointOff()
    {
        theRend.material = cpOff;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            playerDeathManager.SetSpawnPoint(transform.position);
            CheckpointOn();
        }
    }

}
