using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicLayering : MonoBehaviour
{
    //TODO: Add sliders for starting volume, pan and other properties
    public int layer = 0;
    public AudioMixerGroup output;
    public bool DisableControl = true;
    public AudioClip[] layerClips;
    public bool[] layerOnStart;
    public List<AudioSource> layerSources;
    public List<int> FadeOutSources;
    public List<int> FadeInSources;
    public float FadeOutTime;
    public float FadeInTime;

    void Start()
    {
        for (int i = 0; i < layerClips.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = layerClips[i];
            source.loop = true;
            source.outputAudioMixerGroup = output;
            source.Play();
            if (layerOnStart[i])
            {
                source.volume = 1;
            }
            else
            {
                source.volume = 0;
            }
            layerSources.Add(source);
        }

    }

    private void Update()
    {
        foreach (int i in FadeOutSources)
        {
            layerSources[i].volume = Mathf.Lerp(layerSources[i].volume, 0f, FadeOutTime * Time.deltaTime);
        }
        foreach (int i in FadeInSources)
        {
            layerSources[i].volume = Mathf.Lerp(layerSources[i].volume, 1f, FadeInTime * Time.deltaTime);
        }
    }
    public IEnumerator TrackSwitch(bool on, int layer)
    {
        yield return new WaitForSeconds(layerSources[layer].clip.length - layerSources[layer].time); //Wait until the end of the clip
        if (on)
        {
            layerSources[layer].volume = 1;
        }
        else
        {
            layerSources[layer].volume = 0;
        }
    }

    public void AddFadeOut(int layer)
    {
        FadeOutSources.Add(layer);
        FadeInSources.Remove(layer);
    }
    public void AddFadeIn(int layer)
    {
        FadeInSources.Add(layer);
        FadeOutSources.Remove(layer);
    }
    public void StartClip(int layer)
    {
        StartCoroutine(TrackSwitch(true, layer));
    }
    public void StopClip(int layer)
    {
        StartCoroutine(TrackSwitch(false, layer));
    }
    public void PlayOnce(int layer)
    {
        FadeInSources.Remove(layer);
        FadeOutSources.Remove(layer);
        layerSources[layer].loop = false;
        layerSources[layer].volume = 1;
        layerSources[layer].Play();
    }
}
