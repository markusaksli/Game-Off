using System.Collections;
using UnityEngine;

public class Rifle : MonoBehaviour
{

    public int gunDamage = 1;                                            // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 1f;                                        // Number in seconds which controls how often the player can fire
    public float weaponRange = 5000f;                                        // Distance in Unity units over which the player can fire
    public float hitForce = 1000f;                                        // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                                // Float to store the time the player will be allowed to fire again, after firing


    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunEnd = GameObject.FindGameObjectWithTag("GunEnd").transform;
    }


    void Update()
    {
        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            // Update the time when our player can fire next
            nextFire = Time.time + fireRate;

            StartCoroutine(ShotEffect());

            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition(0, gunEnd.position);

            // Check if our raycast has hit anything
            if (Physics.Raycast(gunEnd.position, gunEnd.forward * weaponRange, out hit, weaponRange))
            {
                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null && hit.rigidbody.tag == "Enemy")
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition(1, gunEnd.position + (gunEnd.forward * weaponRange));
            }
        }
    }


    private IEnumerator ShotEffect()
    {
        laserLine.startColor = new Color(1, 0, 0);
        laserLine.endColor = new Color(1, 0, 0);

        yield return shotDuration;

        laserLine.startColor = new Color(1, 1, 1);
        laserLine.endColor = new Color(1, 1, 1);
    }
}
