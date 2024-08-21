using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] private GameObject fpsCamera;
    [SerializeField] private GameObject tpsCamera;
    bool thirdPerson;
    public PhotonView view;

    private void Update() {
        if(!view.IsMine) return;
        fpsCamera.SetActive(!thirdPerson);
        tpsCamera.SetActive(thirdPerson);
        tpsCamera.transform.position = fpsCamera.transform.position;
        tpsCamera.transform.eulerAngles = new Vector3(fpsCamera.transform.eulerAngles.x, fpsCamera.transform.eulerAngles.y, 0);
    }

    public void TogglePerspective(bool toggleTPS) {
        thirdPerson = toggleTPS;
    }
}
