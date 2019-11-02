using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float InputX, InputZ, Speed;
    private CharacterController CC;
    private Camera cam;
    private Vector3 desLookDir;
    private Vector3 desMoveDir;

    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float allowRotation = 0.1f;
    [SerializeField] float gravity;

    void Start()
    {
        CC = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        InputDecider();
    }

    void InputDecider()
    {
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        if (Speed > allowRotation)
        {
            PlayerRotation();
            PlayerMovement();
        }
        else
        {
            desMoveDir = Vector3.zero;
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
        if (CC.isGrounded)
        {
            gravity = 0;
        }
        else
        {
            gravity -= 9.8f * Time.deltaTime;
        }

        desMoveDir = transform.forward.normalized * (Time.deltaTime * moveSpeed);
        desMoveDir = new Vector3(desMoveDir.x, gravity, desMoveDir.z);
        CC.Move(desMoveDir);

    }
}
