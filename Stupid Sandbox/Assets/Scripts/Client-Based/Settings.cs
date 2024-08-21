using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;
    public TMP_Dropdown qualityDropdown;
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider SensSlider;
    public Toggle InGameTooltips;
    public Toggle mobileControls;
    public Transform MenuTransform;
    public GameObject[] inGameTooltipsGameObject;
    public GameObject mobileControlsGameObject;

    public void SetMasterVolume(float volume) {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("MasterVolume"));
    }
    public void SetMusicVolume(float volume) {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
    }
    public void SetSFXVolume(float volume) {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVolume"));
    }
    public void SetQuality(int idx) {
        PlayerPrefs.SetInt("Quality", idx);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
    }
    public void SetSens(float amount) {
        PlayerPrefs.SetFloat("Sens", amount);
    }
    public void SetInGameTooltips(bool toggle) {
        int idx = 1;
        if (toggle) {
            idx = 1;
        }
        else {
            idx = 0;
        }
        PlayerPrefs.SetInt("Tooltips", idx);
        if (inGameTooltipsGameObject != null) {
            inGameTooltipsGameObject[0].SetActive(toggle);
            inGameTooltipsGameObject[1].SetActive(!toggle);
        }
    }
    public void SetMobileControls(bool toggle) {
        int idx = 1;
        if (toggle) {
            idx = 1;
        }
        else {
            idx = 0;
        }
        PlayerPrefs.SetInt("Mobile", idx);
        if (mobileControlsGameObject != null) {
            mobileControlsGameObject.SetActive(toggle);
        }
    }
    private void OnEnable() {
        if (!PlayerPrefs.HasKey("MasterVolume")) {
            Reset();
        }
        if (!PlayerPrefs.HasKey("MusicVolume")) {
            Reset();
        }
        if (!PlayerPrefs.HasKey("SFXVolume")) {
            Reset();
        }
        if (!PlayerPrefs.HasKey("Sens")) {
            Reset();
        }
        if (!PlayerPrefs.HasKey("Tooltips")) {
            Reset();
        }
        if (!PlayerPrefs.HasKey("Mobile")) {
            Reset();
        }
    }
    public void Reset() {
        MasterSlider.value = 0;
        PlayerPrefs.SetFloat("MasterVolume", 0);
        audioMixer.SetFloat("Master", 0);
        MusicSlider.value = 0;
        PlayerPrefs.SetFloat("MusicVolume", 0);
        audioMixer.SetFloat("Music", 0);
        SfxSlider.value = 0;
        PlayerPrefs.SetFloat("SFXVolume", 0);
        audioMixer.SetFloat("SFX", 0);
        SensSlider.value = 1.5f;
        PlayerPrefs.SetFloat("Sens", 0.7f);
        InGameTooltips.isOn = false;
        PlayerPrefs.SetFloat("Tooltips", 0);
        mobileControls.isOn = Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android;
        PlayerPrefs.SetInt("Mobile", Convert.ToInt16(Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android));
    }
    private void Awake() {
        qualityDropdown.value = PlayerPrefs.GetInt("Quality");
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("MasterVolume"));
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
        SfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVolume"));
        SensSlider.value = PlayerPrefs.GetFloat("Sens");
        if (PlayerPrefs.HasKey("Tooltips")) {
            InGameTooltips.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("Tooltips"));
            if (inGameTooltipsGameObject != null) {
                inGameTooltipsGameObject[0].SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("Tooltips")));
                inGameTooltipsGameObject[1].SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("Tooltips")));
            }
        }
        else {
            InGameTooltips.isOn = false;
            if (inGameTooltipsGameObject != null) {
                inGameTooltipsGameObject[0].SetActive(false);
                inGameTooltipsGameObject[1].SetActive(true);
            }
        }
        if (PlayerPrefs.HasKey("Mobile")) {
            mobileControls.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("Mobile"));
            if (mobileControlsGameObject != null) {
                mobileControlsGameObject.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("Mobile")));
            }
        }
        else {
            mobileControls.isOn = Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android;
            if (mobileControlsGameObject != null) {
                mobileControlsGameObject.SetActive(Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);
            }
        }
        if (MenuTransform != null) {
            transform.SetParent(MenuTransform);
        }
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    public void OpenSite(string _url) {
        Application.OpenURL(_url);
    }

    private void LateUpdate() {
        PlayerPrefs.Save();
    }
}
