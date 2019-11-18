using UnityEngine;

public class Button : MonoBehaviour
{
    // Press F to open UI Panel element
    public GameObject instructions;
    
    // Check if the Player is in the button's collider
    public void OnTriggerStay(Collider other)
    {
        Interact(other);
    }

    //If the Player exits the button's range stop showing instructions
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            instructions.SetActive(false);
        }
    }
    
    //Made this function virtual so other doors could inherit
    //the same behaviour.
    public virtual void Interact(Collider other)
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

    
}
