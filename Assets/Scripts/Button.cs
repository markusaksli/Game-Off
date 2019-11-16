using UnityEngine;

public class Button : MonoBehaviour
{
    // Press F to open UI Panel element
    public GameObject instructions;
    public Animator animator;
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
