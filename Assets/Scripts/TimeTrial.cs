using UnityEngine;

public class TimeTrial : MonoBehaviour
{
    public GameObject instructions;
    public bool TrialStarted = false;
    public bool TrialComplete = false;
    public float trialTime;
    public int buttonCount;
    public int trialButtonCount;
    float time;

    Animator anim;
    public Animator door;
    AudioSource aud;
    public AudioSource trialNoise;
    public AudioSource trialEndNoise;
    public AudioSource doorNoise;

    private void Start()
    {
        instructions.SetActive(false);
        aud = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !TrialComplete && !TrialStarted)
        {
            instructions.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                instructions.SetActive(false);
                TrialStarted = true;
                anim.SetTrigger("Button");
                aud.Play();
                trialNoise.Play();
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

    // If the player has started the trial start counting down.
    // If he runs out of time teleport him back to trial start and
    // reset the timer.
    void Update()
    {
        if (!TrialComplete)
        {
            if (TrialStarted)
            {
                time -= Time.deltaTime;
                if (time < 0f)
                {
                    TrialStarted = false;
                    trialNoise.Stop();
                    trialEndNoise.Play();
                    anim.SetTrigger("Button");
                    aud.Play();
                    return;
                }
                if (trialButtonCount == 0)
                {
                    TrialStarted = false;
                    TrialComplete = true;
                    trialNoise.Stop();
                    trialEndNoise.Play();
                    doorNoise.Play();
                    door.SetTrigger("DoorOpenClose");
                    return;
                }

            }
            else
            {
                time = trialTime;
                trialButtonCount = buttonCount;
            }
        }
    }




}
