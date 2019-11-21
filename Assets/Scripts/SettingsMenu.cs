using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMPro.TMP_Dropdown resolutionsDropdown;

    Resolution[] resolutions;

    private string[] exclusions =
    {
        "832x624",
        "1152x872",
        "1680x1050",
        "1280x800",
        "1280x960"
    };

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;


        // Gets all the possible resolutions and puts them
        // into a TMP_dropdown list.
        // Also sets your current resolution as the dropdown's
        // default value.

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            if (exclusions.Contains(option) == false)
            {
                options.Add(option);
            }


            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

    }

    // Changes resolution depending on the choice in a dropdown.
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }




    // Changes volume based given value.
    // Here I'm changing examplemixer's master volume.
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    // Changes game quality settings based on the choice in a dropdown.
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Changes to windowed or fullscreen.
    public void SetFullscreen(bool IsFullscreen)
    {
        Screen.fullScreen = IsFullscreen;
    }
}
