using UnityEngine;

public class HideOnLow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (QualitySettings.GetQualityLevel() == 0)
        {
            this.gameObject.SetActive(false);
        }

    }
}
