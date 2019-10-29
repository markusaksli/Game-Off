using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLayering : MonoBehaviour
{
    public AudioClip[] layerClips;
    public bool[] layerOnStart;
    public List<AudioSource> layerSources;
    public int layer = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < layerClips.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = layerClips[i];
            source.loop = true;
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
    public void Blend(bool inOut, int layer)
    {
        if (inOut)
        {
            layerSources[layer].volume = Mathf.Lerp(layerSources[layer].volume, 0, 100f * Time.deltaTime);
        }
        else
        {
            layerSources[layer].volume = Mathf.Lerp(layerSources[layer].volume, 1, 100f * Time.deltaTime);
        }
    }
    public IEnumerator TrackSwitch(bool on, int layer)
    {
        yield return new WaitForSeconds(layerSources[layer].clip.length - layerSources[layer].time);
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
