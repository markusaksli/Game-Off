using UnityEngine;

public class Button : MonoBehaviour
{
    // Press F to open UI Panel element
    public GameObject instructions;
    public Animator door;
    public AudioSource doorSource;
    public AudioSource buttonSource;
    Animator animator;
    public bool inRange = false;
    public bool used = false;

    private void Start()
    {
        instructions.SetActive(false);
        buttonSource = GetComponentInChildren<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (!used && inRange)
        {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !used)
        {
            inRange = true;
            instructions.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            instructions.SetActive(false);
        }
    }
}
