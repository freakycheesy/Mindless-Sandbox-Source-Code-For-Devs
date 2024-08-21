using MindlessMods;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class Grabbable : MonoBehaviour
{
    [SerializeField]
    public bool isStatic;

    [HideInInspector]
    public PhotonView photonView;
    PhotonTransformView transformView;
    PhotonRigidbodyView rigidbodyView;

    private void Start() {
        photonView = GetComponent<PhotonView>();
        if(photonView == null) photonView = GetComponentInParent<PhotonView>();
        transformView = GetComponent<PhotonTransformView>();
        rigidbodyView = GetComponent<PhotonRigidbodyView>();
        transformView.m_SynchronizePosition = false;
        transformView.m_SynchronizeRotation = false;
        transformView.m_SynchronizeScale = true;
        if (isStatic) {
            rigidbodyView.m_TeleportEnabled = true;
            rigidbodyView.m_SynchronizeVelocity = false;
            rigidbodyView.m_SynchronizeAngularVelocity = false;
        }
    }

    private void Update() {
        if (isStatic) {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = true;
        }
        else if (FindAnyObjectByType<ModSaveSystem>() != null) {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
