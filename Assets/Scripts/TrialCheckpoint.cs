using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrialCheckpoint : MonoBehaviour
{
    private bool HasPassed = false;
    public GameObject timer;
    private Image time;
    private GameObject player;
    public static bool Completed = false;

    // Check if the player has entered the exit or just a checkpoint.
    // Give them extra time if it's a checkpoint.
    // To teleport the player you gotta set make it's gameobject inactive and
    // then back to active right after teleporting.
    public void OnTriggerEnter(Collider other)
    {
        time = timer.GetComponent<Image>();

        if (HasPassed == false && Completed == false)
        {        
            time.fillAmount += 0.2f;
        }

        if (gameObject.tag == "TrialExit")
        {
                       
            if (other.tag == "Player")
            {
          
                other.gameObject.SetActive(false);
                other.transform.position = TimeTrial.startPos;
                other.gameObject.SetActive(true);

                Completed = true;
                time.fillAmount = 1.0f;
                timer.SetActive(false);
            }
        }

        
    }

    //Reset checkpoints if player fails the trial
    public void Update()
    {
        time = timer.GetComponent<Image>();
        if (time.fillAmount < 0.01f)
        {
            HasPassed = false;
        }
    }


    // Make sure the the player couldn't get extra time from  one checkpoint
    // more than once.
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            HasPassed = true;

        }
    }

    
}
