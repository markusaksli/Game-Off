using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrialCheckpoint : MonoBehaviour
{
    private bool HasPassed = false;
    public GameObject timer;
    private Image time;
    public static bool Completed = false;
    public GameObject instructions;

    // Check if the player has entered the exit or just a checkpoint.
    // Give them extra time if it's a checkpoint.
    // To teleport the player you gotta set make it's gameobject inactive and
    // then back to active right after teleporting.
    public void OnTriggerStay(Collider other)
    {
        time = timer.GetComponent<Image>();

        if (HasPassed == false && Completed == false)
        {
            instructions.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                
                time.fillAmount += 0.2f;
                HasPassed = true;
            }        
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
            instructions.SetActive(false);
        }
    }

    
}
