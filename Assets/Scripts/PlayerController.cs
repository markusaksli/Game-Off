using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float InputX, InputZ, Speed;
    private CharacterController CC;
    private Camera cam;
    private Animator anim;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;
    private float gravity;

    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float maxGravity = 100f;
    [SerializeField] float accGravity = 9.8f;

    void Start()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        Gravity();

        if (Input.GetMouseButton(1))
        {
            anim.SetBool("Aiming", true);
        }
        else
        {
            anim.SetBool("Aiming", false);
        }
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        InputDecider();
    }

    void Gravity()
    {
        if (CC.isGrounded)
        {
            gravity = -0.5f * Time.deltaTime;
        }
        else
        {
            if (gravity > -maxGravity)
            {
                gravity -= accGravity * Time.deltaTime;
            }
        }
        CC.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        if (gravity < -3f)
        {
            anim.SetBool("Grounded", false);
        }
        else
        {
            anim.SetBool("Grounded", true);
        }
    }

    void InputDecider()
    {
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        if (Speed > allowRotation)
        {
            PlayerRotation();
            PlayerMovement();
            anim.SetFloat("InputMagnitude", Speed);
        }
        else
        {
            desMoveDir = Vector3.zero;
            anim.SetFloat("InputMagnitude", Speed);
        }
    }

    void PlayerRotation()
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        right.Normalize();
        forward.Normalize();

        desLookDir = forward * InputZ + right * InputX;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desLookDir), rotationSpeed);
    }

    void PlayerMovement()
    {
        desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed);
        desMoveDir.y = 0;
        CC.Move(desMoveDir);
    }
}
