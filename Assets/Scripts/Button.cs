using UnityEngine;

public class Button : MonoBehaviour
{
    // Press F to open UI Panel element
    public GameObject instructions;
    private void Start()
    {
        instructions.SetActive(false);
    }
    // Check if the Player is in the button's collider
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            instructions.SetActive(true);
            Animator animator = GetComponentInChildren<Animator>();
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("DoorOpenClose");
            }
        }
    }

    //If the Player exits the button's range stop showing instructions
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            instructions.SetActive(false);
        }
    }
}
