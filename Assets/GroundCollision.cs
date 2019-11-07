using UnityEngine;

public class GroundCollision : MonoBehaviour
{
    public PlayerController PC;
    bool isGrounded;
    // Update is called once per frame
    void Update()
    {
        PC.isGrounded = isGrounded;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != "Player")
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag != "Player")
        {
            isGrounded = false;
        }
    }

}
