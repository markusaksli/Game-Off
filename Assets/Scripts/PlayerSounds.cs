using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{
    [Range(0f, 1f)] public float footstepVolume;
    [Range(0f, 1f)] public float jumpVolume;
    [Range(0f, 1f)] public float minLandVolume;
    [Range(0f, 1f)] public float maxLandVolume;
    [Range(0f, 1f)] public float flyVolume;
    [Range(0f, 1f)] public float gliderEquipVolume;
    [Range(0f, 1f)] public float gliderHolsterVolume;
    public float flySmooth;
    public AudioMixerGroup output;
    public AudioClip flyClip;
    public AudioClip gliderEquipClip;
    public AudioClip gliderHolsterClip;
    public AudioClip[] jumpSounds;
    public AudioClip[] earthSounds;
    public AudioClip[] woodSounds;
    public List<AudioSource> sources;
    public AudioSource flySource;
    private PlayerController PC;
    private Animator anim;
    int lastStep;
    int lastJump;
    int lastLand;
    float lastCurve;
    float currentCurve;

    private void Update()
    {
        currentCurve = anim.GetFloat("FootstepCurve");
        if ((currentCurve > 0 && lastCurve < 0) || (currentCurve < 0 && lastCurve > 0))
        {
            PlayerStep();
        }
        lastCurve = currentCurve;

        if (anim.GetBool("Flying"))
        {
            flySource.volume = Mathf.Lerp(flySource.volume, flyVolume, flySmooth * Time.deltaTime);
        }
        else
        {
            flySource.volume = Mathf.Lerp(flySource.volume, 0f, flySmooth * Time.deltaTime);
        }
    }

    private void Start()
    {
        PC = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        flySource = gameObject.AddComponent<AudioSource>();
        flySource.clip = flyClip;
        flySource.loop = true;
        flySource.spatialBlend = 1f;
        flySource.reverbZoneMix = 1f;
        flySource.outputAudioMixerGroup = output;
        flySource.volume = 0f;
        flySource.Play();

        for (int i = 0; i < 4; i++)
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
                    step = Random.Range(0, 4);
                    if (step != lastStep)
                    {
                        lastStep = step;
                        break;
                    }
                }
                sc.volume = footstepVolume;
                sc.clip = sounds[step];
                sc.Play();
                return;
            }
        }
    }
    void PlayJump()
    {
        foreach (AudioSource sc in sources)
        {
            if (!sc.isPlaying)
            {
                int jump = 0;
                while (true)
                {
                    jump = Random.Range(0, jumpSounds.Length - 1);
                    if (jump != lastJump)
                    {
                        lastJump = jump;
                        break;
                    }
                }
                sc.volume = jumpVolume;
                sc.clip = jumpSounds[jump];
                sc.Play();
                return;
            }
        }
    }

    void PlayGlider(int holster)
    {
        foreach (AudioSource sc in sources)
        {
            if (!sc.isPlaying)
            {
                if (holster == 1)
                {
                    sc.volume = gliderHolsterVolume;
                    sc.clip = gliderHolsterClip;
                    sc.Play();
                    return;
                }
                else
                {
                    sc.volume = gliderEquipVolume;
                    sc.clip = gliderEquipClip;
                    sc.Play();
                    return;
                }
            }
        }
    }

    void PlayLand(AudioClip[] sounds)
    {
        foreach (AudioSource sc in sources)
        {
            if (!sc.isPlaying)
            {
                int land = 0;
                while (true)
                {
                    land = Random.Range(0, 4);
                    if (land != lastLand)
                    {
                        lastLand = land;
                        break;
                    }
                }
                sc.volume = minLandVolume + (maxLandVolume - minLandVolume) * PC.lastGrav;
                sc.clip = sounds[land + 5];
                sc.Play();
                return;
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
                break;
        }
    }
    public void PlayerLand()
    {
        string tag = PC.groundTag;
        switch (tag)
        {
            case "Earth":
                PlayLand(earthSounds);
                break;
            case "Wood":
                PlayLand(woodSounds);
                break;

            default:
                break;
        }
    }
}
