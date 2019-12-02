using UnityEngine;


public class TrialCheckpoint : MonoBehaviour
{
    public TimeTrial TR;
    public GameObject instructions;
    bool pressed;

    AudioSource aud;
    Animator anim;

    private void Start()
    {
        instructions.SetActive(false);
        aud = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !TR.TrialComplete && TR.TrialStarted && !pressed)
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
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            instructions.SetActive(false);
        }
    }


    public void Update()
    {
        if (!TR.TrialComplete && !TR.TrialStarted && pressed)
        {
            anim.SetTrigger("Button");
            aud.Play();
            pressed = false;
        }
    }


}
