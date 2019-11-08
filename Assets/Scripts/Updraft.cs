using UnityEngine;

public class Updraft : MonoBehaviour
{
    public float UpdraftForce;
    public PlayerController PC;
    public bool UpdraftActive;

    private float RefGravity;

    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        UpdraftActive = false;
        RefGravity = PC.flyGravity;
    }


    void Update()
    {
        if (UpdraftActive && PC.currentState == PlayerController.PlayerState.Flying)
        {
            PC.flyGravity = UpdraftForce;

            if (PC.flyMeter < 3) PC.flyMeter += 3f * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            UpdraftActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UpdraftActive = false;
            PC.flyGravity = RefGravity;
        }
    }
}
