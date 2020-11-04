using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class GameEnd : MonoBehaviour
{
    public PlayerController PC;
    public AudioMixer mixer;
    public UIView view;
    public UIView fadeView;
    bool collided = false;
    float currentVolume;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PC.enableMovement = false;
            collided = true;
            view.Show();
            fadeView.Hide();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (collided)
        {
            mixer.GetFloat("SFXVolume", out currentVolume);
            mixer.SetFloat("SFXVolume", Mathf.Lerp(currentVolume, -80f, Time.deltaTime));
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}
