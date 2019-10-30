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
    //TODO: Create method to Slerp volume between 0 and 1 when called
    /*    public void Blend(bool inOut, int layer)
        {
            if (inOut)
            {
                layerSources[layer].volume = Mathf.Slerp(layerSources[layer].volume, 0, 100f * Time.deltaTime);
            }
            else
            {
                layerSources[layer].volume = Mathf.Slerp(layerSources[layer].volume, 1, 100f * Time.deltaTime);
            }
        }*/
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
    private void Update()
    {
        if (!DisableControl)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(TrackSwitch(false, layer));
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(TrackSwitch(true, layer));
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                layer += 1;
                if (layer == 3)
                {
                    layer = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                layer -= 1;
                if (layer == -1)
                {
                    layer = 2;
                }
            }
        }
    }
}
