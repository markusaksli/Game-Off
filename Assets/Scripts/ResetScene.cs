using UnityEngine;

public class ResetScene : MonoBehaviour
{
    private PlayerController Player;

    private void Start()
    {
        Player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player.Respawn();
        }
    }
}
