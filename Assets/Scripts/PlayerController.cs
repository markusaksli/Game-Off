using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float InputX, InputZ, Speed, jumpVelocity, tempSpeed;
    private CharacterController CC;
    private Camera cam;
    private Animator anim;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;
    private bool justJumped = false;
    private bool flyNext = false;
    public float tempMoveSpeed;
    public float gravity;
    Transform chest;
    public Transform aimPoint;
    public LineRenderer aimLine;
    Vector3 gravVector;
    //Player States
    public enum PlayerState { Default, Aiming, Rolling, Flying, Falling, Jumping };
    public PlayerState currentState;

    //Movement variables
    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float jumpMoveSpeed = 1f;
    [SerializeField] float sprintSpeed = 1f;
    [SerializeField] float flySpeed = 1f;
    [SerializeField] float moveSmooth = 1f;
    [SerializeField] float stopSmooth = 1f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float maxGravity = 100f;
    [SerializeField] float flyGravity = 100f;
    [SerializeField] float maxJumpVelocity = 100f;
    [SerializeField] float jumpVelocityAcc = 100f;
    [SerializeField] float accGravity = 9.8f;
    [SerializeField] Vector3 Offset;

    void Start()
    {
        aimLine.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentState = PlayerState.Default;
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        chest = anim.GetBoneTransform(HumanBodyBones.Chest);
        cam = Camera.main;
    }

    void Update()
    {
        //Constantly update Input values
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        InputDecider();
    }


    void InputDecider()
    {
        //Start aiming if in default
        if (currentState == PlayerState.Default)
        {
            if (Input.GetMouseButton(1))
            {
                anim.SetBool("Aiming", true);
                currentState = PlayerState.Aiming;
            }
        }
        //Jump and Fly
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hitJump;
            Physics.Raycast(transform.position, Vector3.down, out hitJump);
            //Only jump if in default
            if (currentState == PlayerState.Default)
            {
                if (Vector3.Angle(transform.up, hitJump.normal) <= CC.slopeLimit)
                {
                    anim.SetTrigger("Jump");
                    currentState = PlayerState.Jumping;
                    flyNext = false;
                }
            }
            //Start falling if flying
            else if (currentState == PlayerState.Flying)
            {
                currentState = PlayerState.Falling;
            }
            //Start flying if falling
            else if (currentState == PlayerState.Falling)
            {
                currentState = PlayerState.Flying;
            }
        }

        //Choose logic based on PlayerState
        switch (currentState)
        {
            case PlayerState.Default:
                Gravity();
                anim.SetBool("Aiming", false);
                anim.SetBool("Flying", false);
                anim.SetBool("Grounded", true);
                jumpVelocity = 0;
                DefaultMovement();
                break;

            case PlayerState.Aiming:
                Gravity();
                aimLine.enabled = true;
                anim.SetBool("Grounded", true);
                anim.SetBool("Aiming", true);
                if (!Input.GetMouseButton(1))
                {
                    anim.SetBool("Aiming", false);
                    aimLine.enabled = false;
                    currentState = PlayerState.Default;
                }
                AimMovement();
                break;

            case PlayerState.Rolling:
                break;

            case PlayerState.Flying:
                anim.SetBool("Flying", true);
                DefaultMovement();
                Fly();
                break;

            case PlayerState.Jumping:
                Jump();
                DefaultMovement();
                break;

            case PlayerState.Falling:
                Gravity();
                justJumped = false;
                anim.SetBool("Grounded", false);
                anim.SetBool("Flying", false);
                DefaultMovement();
                break;
        }

    }
    //TODO: Implement Shooting, Rolling, Damage and Death states and logic

    void Gravity() //Used to glue the player to the ground and control falling
    {
        //TODO: Damage/Death
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (hit.collider)
        {
            if (Vector3.Angle(transform.up, hit.normal) <= CC.slopeLimit)
            {
                if (hit.distance > 0.5f)
                {
                    if (gravity > 0)
                    {
                        gravity = -1;
                    }
                    if (gravity > -maxGravity)
                    {
                        gravity -= accGravity * Time.deltaTime;
                    }
                    CC.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
                    currentState = PlayerState.Falling;
                }
                else
                {
                    gravity = 10f;
                    if (currentState == PlayerState.Falling)
                    {
                        gravVector = Vector3.zero;
                        currentState = PlayerState.Default;
                        desMoveDir = Vector3.zero;
                        tempMoveSpeed = 0;
                    }
                    CC.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
                }
            }
            else
            {
                float tempDist = Vector3.Angle(transform.up, hit.normal) / 90 * 30;
                Debug.Log(tempDist);
                if (hit.distance < tempDist)
                {
                    gravVector.x += (1f - hit.normal.y) * hit.normal.x * 0.7f;
                    gravVector.z += (1f - hit.normal.y) * hit.normal.z * 0.7f;
                    gravVector.y -= 10f;
                    CC.Move(gravVector * Time.deltaTime);
                }
            }
        }
        else
        {
            if (gravity > 0)
            {
                gravity = -1;
            }
            if (gravity > -maxGravity)
            {
                gravity -= accGravity * Time.deltaTime;
            }
            CC.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
            currentState = PlayerState.Falling;
        }
    }

    void DefaultMovement() //Used for movement while not aiming and while mid air
    {
        Speed = new Vector2(InputX, InputZ).normalized.sqrMagnitude; //Start moving if input is above Deadzone threshold
        if (Speed > allowRotation)
        {
            //Use camera do determine rotation
            var forward = cam.transform.forward;
            var right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            right.Normalize();
            forward.Normalize();

            desLookDir = forward * InputZ + right * InputX;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), rotationSpeed*Time.deltaTime); //Rotate towards movement input

            //Choose movement speed based on PlayerState and sprint button
            if (currentState == PlayerState.Default)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, sprintSpeed, moveSmooth * Time.deltaTime);
                    tempSpeed = Mathf.Lerp(tempSpeed, Speed, moveSmooth / 15 * Time.deltaTime);
                    anim.SetFloat("InputMagnitude", tempSpeed);
                }
                else
                {
                    tempMoveSpeed = Mathf.Lerp(tempMoveSpeed, moveSpeed, moveSmooth * Time.deltaTime);
                    tempSpeed = Mathf.Lerp(tempSpeed, Speed / 1.5f, moveSmooth / 15 * Time.deltaTime);
                    anim.SetFloat("InputMagnitude", tempSpeed);
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
            desMoveDir = transform.forward.normalized * tempMoveSpeed;
            desMoveDir.y = 0;
            CC.Move(desMoveDir * Time.deltaTime); //Move forward at movement speed
        }
        else //If input is below threshold, zero movement
        {
            tempMoveSpeed = 0;
            desMoveDir = Vector3.Slerp(desMoveDir, Vector3.zero, stopSmooth * Time.deltaTime);
            tempSpeed = Mathf.Lerp(tempSpeed, Speed, stopSmooth * Time.deltaTime);
            anim.SetFloat("InputMagnitude", tempSpeed);
        }
    }

    void AimMovement()
    {
        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, lookX * Time.deltaTime * rotationSpeed * 1000);

        aimPoint.Translate(Vector3.up * lookY * Time.deltaTime, Space.World);
        chest.LookAt(aimPoint);
        chest.rotation = chest.rotation * Quaternion.Euler(Offset);

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
            desMoveDir = desMoveDir.normalized * Time.deltaTime * moveSpeed;
            CC.Move(desMoveDir);
        }
        else
        {
            desMoveDir = Vector3.zero;
        }
        anim.SetFloat("InputX", InputX);
        anim.SetFloat("InputY", InputZ);
    }

    void Jump()
    {
        anim.SetBool("Grounded", false);
        if (!justJumped)
        {
            jumpVelocity = maxJumpVelocity;
            justJumped = true;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                flyNext = true;
            }
            if (jumpVelocity < 0)
            {
                if (flyNext)
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
            else
            {
                jumpVelocity -= jumpVelocityAcc * Time.deltaTime;
            }
        }
        CC.Move(new Vector3(0, jumpVelocity * Time.deltaTime, 0));
    }

    void Fly()
    {
        if (gravity > -flyGravity)
        {
            gravity -= accGravity * Time.deltaTime;
        }
        else if (gravity < -flyGravity)
        {
            gravity += accGravity * Time.deltaTime;
        }

        CC.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (!hit.collider)
        {
            return;
        }
        if (hit.distance < 0.5f && Vector3.Angle(transform.up, hit.normal) <= CC.slopeLimit || CC.isGrounded)
        {
            currentState = PlayerState.Default;
            anim.SetBool("Grounded", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Flying", false);
            desMoveDir = Vector3.zero;
            tempMoveSpeed = 0;
        }
    }

}
