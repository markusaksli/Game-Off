﻿using Doozy.Engine.Progress;
using Doozy.Engine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    //References
    private float InputX, InputZ, Speed, jumpVelocity, tempMoveSpeed, glideRotate;
    private CharacterController CC;
    private Camera cam;
    private Animator anim;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;
    private bool justJumped = false;
    private bool flyNext = false;
    private bool spawning;
    RaycastHit hit;
    Progressor flyBar;
    UIView fadeView;
    public AudioMixer audioMixer;
    public TrailRenderer[] gliderTrails;

    [Header("Player Info:")]
    public string groundTag;
    public float flyMeter;
    public float currentAngle;
    public bool sliding;
    public bool isGrounded;
    public PlayerState currentState;
    public enum PlayerState { Default, Aiming, Flying, Falling, Jumping };
    public bool enableMovement;
    public bool enableGravity;
    public bool death;
    public float lastGrav;
    [Space(20)]

    [Header("Gravity:")]
    public float accGravity;
    public float gravitySmooth;
    public float maxGravity;
    public float slideFriction;
    public float flyGravity;
    public float maxFlyMeter;
    Vector3 gravVector;
    float gravity;
    public float castOffset;
    public float castRadius;
    [Space(20)]

    [Header("Movement:")]
    [SerializeField] float allowRotation;
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSmooth;
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpMoveSpeed;
    [SerializeField] float flyRotationSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float stopSmooth;
    [SerializeField] float maxJumpVelocity;
    [SerializeField] float jumpVelocityAcc;
    [Space(20)]

    public Transform aimPoint;
    public LineRenderer aimLine;


    private Vector3 respawnPoint = new Vector3();  //Created a vector3 here for future use when creating respawn

    void Start()
    {
        cam = Camera.main;
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        flyBar = GetComponent<Progressor>();
        fadeView = FindObjectOfType<UIView>();

        currentState = PlayerState.Default;
        aimLine.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (TrailRenderer tr in gliderTrails)
        {
            tr.emitting = false;
        }
    }

    void Update()
    {
        if (!enableMovement)
        {
            InputX = 0;
            InputZ = 0;
        }
        else
        {
            InputX = Input.GetAxis("Horizontal"); //Update Input
            InputZ = Input.GetAxis("Vertical");
        }
        isGrounded = CC.isGrounded;
        CheckCollision();
        InputDecider();
    }

    //Check if slope angle is too great
    void CheckCollision()
    {
        if (isGrounded)
        {
            Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + castOffset, transform.position.z), castRadius, Vector3.down, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject != null)
            {
                groundTag = hit.transform.tag;
            }
            currentAngle = Vector3.Angle(transform.up, hit.normal);
            sliding = (currentAngle > CC.slopeLimit);
        }
    }

    //Get input for state changes and execute state logic
    void InputDecider()
    {
        //Start aiming if right mouse and in default state
        /*        if (currentState == PlayerState.Default)
                {
                    if (Input.GetMouseButton(1))
                    {
                        anim.SetBool("Aiming", true);
                        currentState = PlayerState.Aiming;
                    }

                }*/
        //Jump and Fly
        if (Input.GetKeyDown(KeyCode.Space) && enableMovement)
        {
            //Only jump if in default and not sliding
            if (currentState == PlayerState.Default && !sliding)
            {
                anim.SetTrigger("Jump");
                currentState = PlayerState.Jumping;
                flyNext = false;
            }
            //Start falling if flying
            else if (currentState == PlayerState.Flying)
            {
                currentState = PlayerState.Falling;
            }
            //Start flying if falling
            else if (currentState == PlayerState.Falling && !CC.isGrounded && flyMeter > 0)
            {
                currentState = PlayerState.Flying;
            }
        }

        //Choose logic based on PlayerState
        switch (currentState)
        {
            case PlayerState.Default:
                aimLine.enabled = false;
                if (flyMeter < maxFlyMeter)
                {
                    flyMeter += 3 * Time.deltaTime;
                    flyBar.SetValue(flyMeter);
                }

                anim.SetBool("Aiming", false);
                anim.SetBool("Flying", false);
                anim.SetBool("Grounded", true);

                TrailsStop();

                jumpVelocity = 0;
                CC.Move((Gravity() + DefaultMovement()) * Time.deltaTime); //Move player and execute movement methods
                break;

            case PlayerState.Aiming:
                if (!Input.GetMouseButton(1))
                {
                    anim.SetBool("Aiming", false);
                    currentState = PlayerState.Default;
                    break;
                }
                aimLine.enabled = true;

                anim.SetBool("Grounded", true);
                anim.SetBool("Aiming", true);
                anim.SetFloat("InputX", InputX);
                anim.SetFloat("InputY", InputZ);

                CC.Move((AimMovement() + Gravity()) * Time.deltaTime);
                break;

            case PlayerState.Flying:
                if (flyMeter <= 0)
                {
                    currentState = PlayerState.Falling;
                    break;
                }
                else
                {
                    flyMeter -= Time.deltaTime;
                    flyBar.SetValue(flyMeter);
                }

                anim.SetBool("Flying", true);

                CC.Move((Fly() + DefaultMovement()) * Time.deltaTime);
                break;

            case PlayerState.Jumping:
                CC.Move((Jump() + DefaultMovement()) * Time.deltaTime);
                RaycastHit jumpHit;
                Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + castOffset, transform.position.z), castRadius, Vector3.up, out jumpHit, 0.04f);

                if (jumpHit.normal.y < 0)
                {
                    currentState = PlayerState.Falling;
                }
                break;

            case PlayerState.Falling:
                aimLine.enabled = false;
                justJumped = false;
                anim.SetBool("Grounded", false);
                anim.SetBool("Flying", false);
                CC.Move((Gravity() + DefaultMovement()) * Time.deltaTime);
                break;
        }

    }

    //Draw SphereCast Sphere
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + castOffset, transform.position.z), castRadius);
    }

    Vector3 Gravity() //Used to glue the player to the ground and control falling
    {
        if (isGrounded)
        {
            if (!sliding)
            {
                if (currentState == PlayerState.Falling)
                {
                    lastGrav = Mathf.Abs(gravVector.y / maxGravity);
                    gravVector = Vector3.zero;
                    desMoveDir = Vector3.zero;
                    tempMoveSpeed = 0;
                    currentState = PlayerState.Default;
                    return new Vector3(0, -10f, 0);
                }
                gravity = 10f;
                gravVector = new Vector3(0, -gravity, 0);
            }
            else
            {
                if (gravity == 10)
                {
                    gravVector.y = 0;
                }

                currentState = PlayerState.Falling;
                Vector3 c = Vector3.Cross(hit.normal, Vector3.up);

                gravVector -= Vector3.Cross(c, hit.normal).normalized * slideFriction * Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 4) * Time.deltaTime;

                return gravVector;
            }
        }
        else
        {
            if (gravity == 10)
            {
                gravity = 0;
            }
            if (gravity > -maxGravity)
            {
                gravity -= accGravity * Time.deltaTime;
            }
            currentState = PlayerState.Falling;
            gravVector = new Vector3(0, gravity, 0);
        }
        return gravVector;
    }

    Vector3 DefaultMovement() //Used for movement while not aiming and while mid air
    {
        Speed = new Vector2(InputX, InputZ).normalized.sqrMagnitude; //Start moving if input is above Deadzone threshold
        if (Speed > allowRotation)
        {
            //Use camera to determine rotation
            var forward = cam.transform.forward;
            var right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            right.Normalize();
            forward.Normalize();

            //Rotate towards movement input
            desLookDir = forward * InputZ + right * InputX;
            if (currentState == PlayerState.Flying)
            {
                glideRotate = Mathf.Lerp(glideRotate, InputX, flyRotationSpeed * 2 * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), flyRotationSpeed * Time.deltaTime);
            }
            else
            {
                glideRotate = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), rotationSpeed * Time.deltaTime);
            }
            anim.SetFloat("GlideRotate", glideRotate);

            //Choose movement speed based on PlayerState and sprint button
            if (currentState == PlayerState.Default)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, sprintSpeed, moveSmooth * Time.deltaTime);
                }
                else
                {
                    tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, moveSpeed, moveSmooth * Time.deltaTime);
                }
            }
            else if (currentState == PlayerState.Flying)
            {
                tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, flySpeed, moveSmooth * Time.deltaTime);
            }
            else
            {
                tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, jumpMoveSpeed, moveSmooth * Time.deltaTime);
            }
            anim.SetFloat("InputMagnitude", tempMoveSpeed / sprintSpeed);
            desMoveDir = transform.forward.normalized * tempMoveSpeed;
            desMoveDir.y = 0;
            return desMoveDir;
        }
        else //If input is below threshold, zero movement
        {
            glideRotate = Mathf.Lerp(glideRotate, 0, flyRotationSpeed / 2 * Time.deltaTime);
            anim.SetFloat("GlideRotate", glideRotate);
            tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, 0f, stopSmooth * Time.deltaTime);
            anim.SetFloat("InputMagnitude", tempMoveSpeed / sprintSpeed);
            if ((tempMoveSpeed / sprintSpeed) < 0.01f)
            {
                anim.SetFloat("InputMagnitude", 0);
            }
            else
            {
                anim.SetFloat("InputMagnitude", tempMoveSpeed / sprintSpeed);
            }
            desMoveDir = transform.forward.normalized * tempMoveSpeed;
            desMoveDir.y = 0;
            return desMoveDir;
        }
    }

    Vector3 AimMovement() //Movement while aiming
    {
        //Get mouse input;
        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");
        //Rotate around Y axis
        transform.Rotate(Vector3.up, lookX * Time.deltaTime * rotationSpeed);
        //Move AimPoint up and down
        aimPoint.Translate(Vector3.up * lookY * Time.deltaTime, Space.World);

        //Aiming movement
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        if (Speed > allowRotation)
        {
            var forward = cam.transform.forward;
            var right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            right.Normalize();
            forward.Normalize();
            desMoveDir = forward * InputZ + right * InputX;
            desMoveDir = desMoveDir.normalized * moveSpeed;
        }
        else
        {
            desMoveDir = Vector3.zero;
        }
        return desMoveDir;
    }

    Vector3 Jump() //Jumping movement and exit conditions
    {
        anim.SetBool("Grounded", false);
        if (!justJumped) //Set initial jump velocity
        {
            jumpVelocity = maxJumpVelocity;
            justJumped = true;
        }
        else
        {
            if (isGrounded) //Get out of jump state if collided
            {
                currentState = PlayerState.Falling;
            }
            if (Input.GetKeyDown(KeyCode.Space)) //Fly after jump if space is pressed
            {
                flyNext = true;
            }
            if (jumpVelocity < 0) //If at peak of jump
            {
                if (flyNext && !CC.isGrounded && flyMeter > 0)
                {
                    currentState = PlayerState.Flying;
                    gravity = -1;
                }
                else
                {
                    currentState = PlayerState.Falling;
                }
                justJumped = false;
                flyNext = false;
            }
            else //Decrease velocity
            {
                jumpVelocity -= jumpVelocityAcc * Time.deltaTime;
            }
        }
        return new Vector3(0, jumpVelocity, 0);
    }

    Vector3 Fly()
    {
        if (CC.isGrounded) //Reset and exit if hit something
        {
            currentState = PlayerState.Default;
            anim.SetBool("Grounded", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Flying", false);
            desMoveDir = Vector3.zero;
            tempMoveSpeed = 0;
            return Vector3.zero;
        }

        anim.SetBool("Grounded", false);

        gravity = Mathf.Lerp(gravity, -flyGravity, 5f * Time.deltaTime);

        return new Vector3(0, gravity, 0);
    }

    //Need to create respawnPoint vector3 where according to checkpoint
    //a newPosition is created. When that is added, checkpoints will work.
    public void SetSpawnPoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }

    public void Respawn()
    {
        if (!spawning)
        {
            StartCoroutine(Spawn());
        }
    }

    private IEnumerator Spawn()
    {
        spawning = true;
        float hideWaitTime = fadeView.HideBehavior.Animation.TotalDuration - fadeView.HideBehavior.Animation.StartDelay;
        float showWaitTime = fadeView.ShowBehavior.Animation.TotalDuration - fadeView.ShowBehavior.Animation.StartDelay;

        fadeView.Hide();
        yield return new WaitForSeconds(hideWaitTime);
        float previousVolume;
        audioMixer.GetFloat("SFXVolume", out previousVolume);
        audioMixer.SetFloat("SFXVolume", -80);

        if (!respawnPoint.Equals(new Vector3()))
        {
            CC.enabled = false;
            this.transform.position = respawnPoint;
            fadeView.Show();
            CC.enabled = true;
            yield return new WaitForSeconds(showWaitTime);
        }
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        audioMixer.SetFloat("SFXVolume", previousVolume);
        spawning = false;
        yield return null;
    }

    public void TrailsStart()
    {
        foreach (TrailRenderer tr in gliderTrails)
        {
            tr.emitting = true;
        }
    }
    public void TrailsStop()
    {
        foreach (TrailRenderer tr in gliderTrails)
        {
            tr.emitting = false;
        }
    }
}
