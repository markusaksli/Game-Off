using UnityEngine;

public class Destruct : MonoBehaviour
{
    public GameObject shattered;
    private void OnTriggerEnter(Collider other)
    {
        Instantiate(shattered, this.transform.position, this.transform.rotation, null);
        Destroy(this.gameObject);
    }
}
