using UnityEngine;

public class Button : MonoBehaviour
{
    // Press F to open UI Panel element
    public GameObject instructions;
    public Animator door;
    public AudioSource doorSource;
    public AudioSource buttonSource;
    bool used = false;

    private void Start()
    {
        instructions.SetActive(false);
        buttonSource = GetComponentInChildren<AudioSource>();
    }

    // Check if the Player is in the button's collider
    public void OnTriggerStay(Collider other)
    {
        if (!used)
        {
            Interact(other);
        }
        else
        {
            instructions.SetActive(false);
        }
    }

    //If the Player exits the button's range stop showing instructions
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !used)
        {
            instructions.SetActive(false);
        }
    }

    //Made this function virtual so other doors could inherit
    //the same behaviour.
    public virtual void Interact(Collider other)
    {
        if (other.tag == "Player" && !used)
        {
            instructions.SetActive(true);
            Animator animator = GetComponentInChildren<Animator>();
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("DoorOpenClose");
                door.SetTrigger("DoorOpenClose");
                doorSource.Play();
                buttonSource.Play();
                used = true;
                instructions.SetActive(false);
            }
        }
    }


}
