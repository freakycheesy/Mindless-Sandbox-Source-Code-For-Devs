using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RoomCode : MonoBehaviourPunCallbacks
{
    public TMP_Text textComp;
    NetworkManager networkManager;

    private void Start() {
        textComp = GetComponent<TextMeshPro>();
        networkManager = FindAnyObjectByType<NetworkManager>();
    }

    void Update() {
        if (textComp != null) {
            if (!PhotonNetwork.InRoom) return;
            textComp.text = networkManager.currentRoomCode;
        }
    }
}
