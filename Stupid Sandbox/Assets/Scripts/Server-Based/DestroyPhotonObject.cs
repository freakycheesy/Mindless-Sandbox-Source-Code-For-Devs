using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPhotonObject : MonoBehaviour
{
    [SerializeField] float timeUntilDeletion = 0;
    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        Invoke(nameof(DeleteObject), timeUntilDeletion);
    }

    void DeleteObject() {
        PhotonNetwork.Destroy(view.gameObject);
    }
}
