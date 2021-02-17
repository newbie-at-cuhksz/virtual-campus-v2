using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown dropDownMenu;
    public Slider sliderVolume;
    public AudioSource LobbyMainMenuAudio;
    public List<int> resolutionOptions_h;
    public List<int> resolutionOptions_v;

    void Start()
    {
        sliderVolume.minValue = 0f;
        sliderVolume.maxValue = 1f;
        dropDownMenu.onValueChanged.AddListener(changeValue);
        sliderVolume.onValueChanged.AddListener(changeVolumn);
    }

    void changeValue(int type)
    {
        //print("testing: " + resolutionOptions_h[type] + " " + resolutionOptions_v[type]);
        Screen.SetResolution(resolutionOptions_h[type], resolutionOptions_v[type], false);
    }

    void changeVolumn(float value)
    {
        LobbyMainMenuAudio.volume = value;
    }
}
