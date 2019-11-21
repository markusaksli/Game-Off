using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTrial : MonoBehaviour
{
    public GameObject instructions;
    public GameObject timer;
    public bool StartTrial = false;
    private Image time;
    public static Vector3 startPos;
    
    // Check if player has activated time trial by pressing F
    // next to the start. Then activate timer in player UI Canvas
    // and save starting position so he could be teleported back to start
    // if he fails or succeeds.
    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && TrialCheckpoint.Completed == false)
        {
            instructions.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                timer.SetActive(true);
                StartTrial = true;
                startPos = other.transform.position;
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
        time = timer.GetComponent<Image>();
        
        if (StartTrial == true && TrialCheckpoint.Completed == false)
        {          
            time.fillAmount -= Time.deltaTime / 10.0f;
        }

        if (time.fillAmount <= 0.0f)
        {
            GameObject player = GameObject.Find("Player");
            player.gameObject.SetActive(false);
            player.transform.position = TimeTrial.startPos;
            player.gameObject.SetActive(true);
           
           
            StartTrial = false;
            time.fillAmount = 1.0f;
            timer.SetActive(false);
        }
    }

    


}
