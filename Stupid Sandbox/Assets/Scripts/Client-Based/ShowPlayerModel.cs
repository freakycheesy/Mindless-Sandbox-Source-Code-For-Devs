using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlayerModel : MonoBehaviour {

    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera tpsCamera;
    [SerializeField] private Transform objectHolder;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject[] models;
    [SerializeField] private GameObject[] hideModels;
    [SerializeField] private GameObject[] overlays;
    public PhotonView view;

    private void Update() {
        if (view.IsMine) {
            for (int i = 0; i < models.Length; i++) {
                models[i].gameObject.layer = 6;
            }
            for (int i = 0; i < hideModels.Length; i++) {
                hideModels[i].gameObject.layer = 11;
                if (hideModels[i].GetComponent<SkinnedMeshRenderer>() != null) {
                    hideModels[i].GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                else if (hideModels[i].GetComponent<MeshRenderer>() != null) {
                    hideModels[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            for (int i = 0; i < overlays.Length; i++) {
                overlays[i].gameObject.layer = 10;
            }
            if (head.GetComponent<AudioListener>() == null) {
                head.AddComponent<AudioListener>();
            }
        }
        else {
            if (fpsCamera == null && tpsCamera == null) return;
            for (int i = 0; i < hideModels.Length; i++) {
                hideModels[i].gameObject.SetActive(false);
            }
            Destroy(fpsCamera.gameObject);
            Destroy(tpsCamera.gameObject);
            Destroy(objectHolder.gameObject);
        }
    }
}
