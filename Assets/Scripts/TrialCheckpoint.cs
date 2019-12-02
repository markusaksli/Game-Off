using UnityEngine;


public class TrialCheckpoint : MonoBehaviour
{
    public TimeTrial TR;
    public GameObject instructions;
    bool pressed;
    bool inRange;

    AudioSource aud;
    Animator anim;

    private void Start()
    {
        instructions.SetActive(false);
        aud = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !TR.TrialComplete && TR.TrialStarted && !pressed)
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

    public void Update()
    {
        if (inRange && !TR.TrialComplete && TR.TrialStarted && !pressed)
        {
            instructions.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                anim.SetTrigger("Button");
                aud.Play();
                pressed = true;
                instructions.SetActive(false);
                TR.trialButtonCount -= 1;
            }
        }
        else if (inRange)
        {
            instructions.SetActive(false);
        }

        if (!TR.TrialComplete && !TR.TrialStarted && pressed)
        {
            anim.SetTrigger("Button");
            aud.Play();
            pressed = false;
        }
    }


}
