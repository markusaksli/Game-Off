using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public PlayerController playerDeathManager;

    public Renderer theRend;

    //Materials to tell the difference which one is active.
    public Material cpOff;
    public Material cpOn;


    private void Start()
    {
        playerDeathManager = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        
    }
    //If enters checkpoint, all checkpoints will be switched off
    //and current one will be made active
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
