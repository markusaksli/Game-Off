using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{
    [Range(0f, 1f)] public float footstepVolume;
    public AudioMixerGroup output;
    public AudioClip[] earthSounds;
    public AudioClip[] woodSounds;
    public List<AudioSource> sources;
    private PlayerController PC;
    int lastStep;

    private void Start()
    {
        PC = GetComponent<PlayerController>();
        for (int i = 0; i < 3; i++)
        {
            AudioSource s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.loop = false;
            s.spatialBlend = 1f;
            s.reverbZoneMix = 1f;
            s.outputAudioMixerGroup = output;
            sources.Add(s);
        }
    }

    void PlayStep(AudioClip[] sounds)
    {
        foreach (AudioSource sc in sources)
        {
            if (!sc.isPlaying)
            {
                int step = 0;
                while (true)
                {
                    step = Random.Range(0, sounds.Length - 1);
                    if (step != lastStep)
                    {
                        lastStep = step;
                        break;
                    }
                }
                sc.volume = footstepVolume;
                sc.clip = sounds[step];
                sc.Play();
            }
        }
    }

    public void PlayerStep()
    {
        string tag = PC.groundTag;
        switch (tag)
        {
            case "Earth":
                PlayStep(earthSounds);
                break;
            case "Wood":
                PlayStep(woodSounds);
                break;

            default:
                Debug.Log("No ground tag");
                break;
        }
    }
}
