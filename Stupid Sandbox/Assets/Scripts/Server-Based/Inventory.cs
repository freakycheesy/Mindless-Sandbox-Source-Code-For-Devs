using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Inventory : MonoBehaviour
{
    [SerializeField] PhotonView view;
    [SerializeField] PlayerList playerList;

    private void Start() {
        playerList = FindAnyObjectByType<PlayerList>();
    }

    private void Update() {
        if (view != null) return;
        foreach (NetworkPlayer networkPlayer in playerList.GetComponentsInChildren<NetworkPlayer>()) {
            if(networkPlayer.photonView.IsMine) {
                view = networkPlayer.photonView;
            }
        }
    }

    public void SwitchPrimaryWeapon(int selectedWeapon) {
        view.RPC("SwitchPrimaryWeapon", RpcTarget.AllBufferedViaServer, selectedWeapon);
    }

    public void SwitchSecondaryWeapon(int selectedWeapon) {
        view.RPC("SwitchSecondaryWeapon", RpcTarget.AllBufferedViaServer, selectedWeapon);
    }
}
