using Doozy.Engine.Progress;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float InputX, InputZ, Speed, jumpVelocity, tempSpeed, tempMoveSpeed;
    private CharacterController CC;
    private Camera cam;
    private Animator anim;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;
    private bool justJumped = false;
    private bool flyNext = false;
    public float flyMeter;
    public float maxFlyMeter;
    public bool sliding;
    public bool isGrounded;
    public float currentAngle;
    RaycastHit hit;
    Progressor flyBar;

    public float gravity;
    public float gravitySmooth;
    public Transform aimPoint;
    public LineRenderer aimLine;
    Vector3 gravVector;
    Vector3 tempGravVector;
    //Player States
    public enum PlayerState { Default, Aiming, Flying, Falling, Jumping };
    public PlayerState currentState;
    public LayerMask groundMask;

    //Movement variables
    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float flyRotationSpeed = 0.3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float jumpMoveSpeed = 1f;
    [SerializeField] float sprintSpeed = 1f;
    [SerializeField] float flySpeed = 1f;
    [SerializeField] float moveSmooth = 1f;
    [SerializeField] float stopSmooth = 1f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float maxGravity = 100f;
    [SerializeField] public float flyGravity = 100f;
    [SerializeField] float maxJumpVelocity = 100f;
    [SerializeField] float jumpVelocityAcc = 100f;
    [SerializeField] float accGravity = 9.8f;
    [SerializeField] float slideFriction = 9.8f;
    [SerializeField] float castOffset = 9.8f;
    [SerializeField] float castRadius = 9.8f;

    void Start()
    {
        aimLine.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentState = PlayerState.Default;
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        flyBar = GetComponent<Progressor>();
        cam = Camera.main;
    }

    void Update()
    {
        //Constantly update Input values
        isGrounded = CC.isGrounded;
        CheckCollision();
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        InputDecider();
    }

    void CheckCollision()
    {
        if (isGrounded)
        {
            Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + castOffset, transform.position.z), castRadius, Vector3.down, out hit, 0.55f);
            currentAngle = Vector3.Angle(transform.up, hit.normal);
            sliding = (currentAngle > CC.slopeLimit);
        }
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
            //Only jump if in default
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
            else if (currentState == PlayerState.Falling && !CC.isGrounded)
            {
                currentState = PlayerState.Flying;
            }
        }

        //Choose logic based on PlayerState
        switch (currentState)
        {
            case PlayerState.Default:
                if (flyMeter < maxFlyMeter)
                {
                    flyMeter += 3 * Time.deltaTime;
                    flyBar.SetValue(flyMeter);
                }
                anim.SetBool("Aiming", false);
                anim.SetBool("Flying", false);
                anim.SetBool("Grounded", true);
                jumpVelocity = 0;
                CC.Move((Gravity() + DefaultMovement()) * Time.deltaTime);
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

            case PlayerState.Flying:
                if (flyMeter <= 0)
                {
                    Debug.Log("Meter ran out");
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
                break;

            case PlayerState.Falling:
                justJumped = false;
                anim.SetBool("Grounded", false);
                anim.SetBool("Flying", false);
                CC.Move((Gravity() + DefaultMovement()) * Time.deltaTime);
                break;
        }

    }
    //TODO: Implement Damage and Death states and logic
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + castOffset, transform.position.z), castRadius);
    }

    Vector3 Gravity() //Used to glue the player to the ground and control falling
    {
        //TODO: Damage/Death
        if (isGrounded)
        {
            if (!sliding)
            {
                if (currentState == PlayerState.Falling)
                {
                    gravVector = Vector3.zero;
                    tempGravVector = Vector3.zero;
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
                /*gravVector.y = -200f * currentAngle / 90;
                gravVector.x += (1f - hit.normal.y) * hit.normal.x * slideFriction * (90 / currentAngle) * Time.deltaTime;
                gravVector.z += (1f - hit.normal.y) * hit.normal.z * slideFriction * (90 / currentAngle) * Time.deltaTime;*/
                /*if (gravVector.y > -maxGravity * currentAngle / 90)
                {
                    gravVector.y -= currentAngle / 90 * accGravity * Time.deltaTime;
                }*/
                currentState = PlayerState.Falling;
                Vector3 c = Vector3.Cross(hit.normal, Vector3.up);
                gravVector = -Vector3.Cross(c, hit.normal).normalized * slideFriction * Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 4);
                if (gravVector.y < -maxGravity)
                {
                    gravVector.y = -maxGravity;
                }
                return gravVector;
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
            currentState = PlayerState.Falling;
            gravVector = new Vector3(0, gravity, 0);
        }
        //tempGravVector = Vector3.Slerp(tempGravVector, gravVector, gravitySmooth * Time.deltaTime);
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

            desLookDir = forward * InputZ + right * InputX;
            if (currentState == PlayerState.Flying)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), flyRotationSpeed * Time.deltaTime); //Rotate towards movement input
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), rotationSpeed * Time.deltaTime); //Rotate towards movement input
            }

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
            return desMoveDir;
        }
        else //If input is below threshold, zero movement
        {
            tempMoveSpeed = 0;
            desMoveDir = Vector3.Slerp(desMoveDir, Vector3.zero, stopSmooth * Time.deltaTime);
            tempSpeed = Mathf.Lerp(tempSpeed, Speed, stopSmooth * Time.deltaTime);
            anim.SetFloat("InputMagnitude", tempSpeed);
            return desMoveDir;
        }
    }

    void AimMovement()
    {
        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, lookX * Time.deltaTime * rotationSpeed);

        aimPoint.Translate(Vector3.up * lookY * Time.deltaTime, Space.World);

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

    Vector3 Jump()
    {
        anim.SetBool("Grounded", false);
        if (!justJumped)
        {
            jumpVelocity = maxJumpVelocity;
            justJumped = true;
        }
        else
        {
            if (isGrounded)
            {
                currentState = PlayerState.Falling;
            }
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
        return new Vector3(0, jumpVelocity, 0);
    }

    Vector3 Fly()
    {
        if (CC.isGrounded)
        {
            currentState = PlayerState.Default;
            anim.SetBool("Grounded", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Flying", false);
            desMoveDir = Vector3.zero;
            tempMoveSpeed = 0;
            return Vector3.zero;
        }

        gravity = Mathf.Lerp(gravity, -flyGravity, 5f * Time.deltaTime);

        return new Vector3(0, gravity, 0);
    }

}
