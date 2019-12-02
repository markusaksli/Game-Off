using UnityEngine;

public class CameraMoveForward : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.forward.normalized * Time.deltaTime);
    }
}
