using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            this.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.L))
        {
            this.transform.Rotate(0, 0, -speed * Time.deltaTime);
        }
    }
}
