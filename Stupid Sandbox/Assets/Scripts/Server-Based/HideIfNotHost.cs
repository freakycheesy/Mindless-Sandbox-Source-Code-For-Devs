using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotHost : MonoBehaviour
{
    private void Update() {
        if (!PhotonNetwork.IsMasterClient) {
            gameObject.SetActive(false);
        }
    }
}
