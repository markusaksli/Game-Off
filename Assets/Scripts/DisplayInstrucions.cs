using UnityEngine;

public class DisplayInstrucions : MonoBehaviour
{
    public GameObject instructions;
    // Start is called before the first frame update
    void Start()
    {
        instructions.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            instructions.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            instructions.SetActive(false);
        }
    }
}
