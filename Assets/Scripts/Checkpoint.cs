using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public PlayerController playerDeathManager;


    private void Start()
    {
        playerDeathManager = FindObjectOfType<PlayerController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDeathManager.SetSpawnPoint(transform.position);
        }
    }

}
