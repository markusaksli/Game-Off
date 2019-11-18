﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{
    [Range(0f, 1f)] public float footstepVolume;
    [Range(0f, 1f)] public float jumpVolume;
    [Range(0f, 1f)] public float landVolume;
    public AudioMixerGroup output;
    public AudioClip[] jumpSounds;
    public AudioClip[] earthSounds;
    public AudioClip[] woodSounds;
    public List<AudioSource> sources;
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
    }

    private void Start()
    {
        PC = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
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
                sc.volume = landVolume;
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