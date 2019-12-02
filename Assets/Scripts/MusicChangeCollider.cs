using UnityEngine;

public class MusicChangeCollider : MonoBehaviour
{
    public bool consume = true;
    public int[] FadeIn;
    public int[] FadeOut;
    public int[] StartLayer;
    public int[] StopLayer;
    public int[] PlayOnceLayer;
    MusicLayering BGM;

    // Start is called before the first frame update
    void Start()
    {
        BGM = FindObjectOfType<MusicLayering>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (int layer in FadeIn)
            {
                BGM.AddFadeIn(layer);
            }

            foreach (int layer in FadeOut)
            {
                BGM.AddFadeOut(layer);
            }

            foreach (int layer in StartLayer)
            {
                BGM.StartClip(layer);
            }

            foreach (int layer in StopLayer)
            {
                BGM.StopClip(layer);
            }

            foreach (int layer in PlayOnceLayer)
            {
                BGM.PlayOnce(layer);
            }

            if (consume)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
