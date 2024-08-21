using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ToggleLight : MonoBehaviour
{
    public InputActionProperty myActionProperty;
    public Light myLight;
    public bool toggled;

    private void Start() {
        myLight = GetComponent<Light>();
        if (!PhotonNetwork.LocalPlayer.IsLocal) {
            Destroy(myLight);
            Destroy(this);
        }
    }

    private void Update() {
        if (myActionProperty.action.WasPressedThisFrame() && PhotonNetwork.LocalPlayer.IsLocal) {
            toggled = !toggled;
            myLight.enabled = toggled;
        }
    }

}
