using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float InputX, InputZ, Speed, jumpVelocity;
    private CharacterController CC;
    private Rigidbody RB;
    private Camera cam;
    private Animator anim;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;
    private bool justJumped = false;
    private bool flyNext = false;
    public float gravity;
    Transform chest;
    public Transform aimPoint;

    public enum PlayerState { Default, Aiming, Rolling, Flying, Falling, Jumping };
    public PlayerState currentState;

    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float maxGravity = 100f;
    [SerializeField] float flyGravity = 100f;
    [SerializeField] float maxJumpVelocity = 100f;
    [SerializeField] float jumpVelocityAcc = 100f;
    [SerializeField] float accGravity = 9.8f;
    [SerializeField] Vector3 Offset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentState = PlayerState.Default;
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        chest = anim.GetBoneTransform(HumanBodyBones.Chest);
        cam = Camera.main;
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
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
            //Only jump if in default
            if (currentState == PlayerState.Default)
            {
                currentState = PlayerState.Jumping;
                flyNext = false;
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


        switch (currentState)
        {
            case PlayerState.Default:
                Gravity();
                anim.SetBool("Aiming", false);
                anim.SetBool("Flying", false);
                jumpVelocity = 0;
                DefaultMovement();
                break;

            case PlayerState.Aiming:
                Gravity();
                anim.SetBool("Aiming", true);
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
                anim.SetBool("Jump", true);
                Jump();
                DefaultMovement();
                break;

            case PlayerState.Falling:
                Gravity();
                justJumped = false;
                anim.SetBool("Jump", false);
                anim.SetBool("Grounded", false);
                anim.SetBool("Flying", false);
                DefaultMovement();
                break;
        }

    }

    void Gravity()
    {
        if (gravity < -maxGravity)
        {
            anim.SetBool("Grounded", false);
            currentState = PlayerState.Falling;
        }
        else
        {
            anim.SetBool("Grounded", true);
        }

        if (CC.isGrounded)
        {
            gravity = -0.1f;
            if (currentState == PlayerState.Falling)
            {
                currentState = PlayerState.Default;
            }
        }
        else
        {
            if (gravity > -maxGravity)
            {
                gravity -= accGravity * Time.deltaTime;
            }
        }
        CC.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
    }

    void DefaultMovement()
    {
        Speed = new Vector2(InputX, InputZ).normalized.sqrMagnitude;
        if (Speed > allowRotation)
        {

            var forward = cam.transform.forward;
            var right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            right.Normalize();
            forward.Normalize();

            desLookDir = forward * InputZ + right * InputX;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), rotationSpeed);

            if (currentState == PlayerState.Default)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed * 1.5f);
                    anim.SetFloat("InputMagnitude", Speed);
                }
                else
                {
                    desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed);
                    Speed = Speed / 1.5f;
                    anim.SetFloat("InputMagnitude", Speed);
                }
            }
            else if (currentState == PlayerState.Flying)
            {
                desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed * 2);
            }
            else
            {
                desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed);
            }
            desMoveDir.y = 0;
            CC.Move(desMoveDir);
        }
        else
        {
            desMoveDir = Vector3.zero;
            anim.SetFloat("InputMagnitude", Speed);
        }
    }

    void AimMovement()
    {
        if (!Input.GetMouseButton(1))
        {
            anim.SetBool("Aiming", false);
            currentState = PlayerState.Default;
        }

        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, lookX * Time.deltaTime * rotationSpeed * 1000);

        aimPoint.Translate(Vector3.up * lookY * Time.deltaTime);
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

        if (CC.isGrounded)
        {
            currentState = PlayerState.Default;
            anim.SetBool("Grounded", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Flying", false);
        }
    }

}
