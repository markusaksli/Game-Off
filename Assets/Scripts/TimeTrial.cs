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
    bool inRange = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !TrialComplete && !TrialStarted)
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


    void Update()
    {
        if (inRange && !TrialComplete && !TrialStarted)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                instructions.SetActive(false);
                TrialStarted = true;
                anim.SetTrigger("Button");
                aud.Play();
                trialNoise.Play();
            }
        }

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
