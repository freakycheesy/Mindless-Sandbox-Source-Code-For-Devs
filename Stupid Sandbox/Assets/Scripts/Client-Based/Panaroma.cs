using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panaroma : MonoBehaviour {
    int selectedMap = 0;
    bool canSwitch = false;
    [SerializeField]
    Animator canvasAnimator;
    [SerializeField]
    GameObject[] maps;
    [SerializeField]
    Material[] skyboxs;

    private void Start() {
        canvasAnimator.Play("Idle");
        Invoke(nameof(canNowSwitch), 10);
    }

    private void Update() {
        if (canSwitch) {
            canSwitch = false;
            canvasAnimator.Play("Splash");
            Invoke(nameof(MapSwitch), 0.45f);
            Invoke(nameof(canNowSwitch), 10);
        }
    }

    void MapSwitch() {
        if (selectedMap == maps.Length - 1) {
            selectedMap = 0;
        }
        else {
            selectedMap++;
        }
        int i = 0;
        foreach (GameObject map in maps) {
            if (i == selectedMap) {
                map.gameObject.SetActive(true);
                RenderSettings.skybox = skyboxs[i];
            }
            else {
                map.gameObject.SetActive(false);
            }
            i++;
        }   
    }

    void canNowSwitch() {
        canSwitch = true;
    }
}
