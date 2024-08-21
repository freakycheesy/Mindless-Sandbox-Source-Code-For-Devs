using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayersInLobby : MonoBehaviour
{
    public int playersIndex;
    public Player[] playersObject;
    public TMP_Text playersUi;
    bool readyToUpdate;
    private void Start() {
        readyToUpdate = true;
        playersIndex = 1;
        playersUi.text = playersIndex.ToString();
        if (PhotonNetwork.OfflineMode) {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (readyToUpdate) {
            readyToUpdate=false;
            playersObject = PhotonNetwork.PlayerList;
            playersIndex = playersObject.Length;
            playersUi.text = playersIndex.ToString();
            Invoke(nameof(ResetReadyToUpdate), 0.5f);
        } 
    }

    private void ResetReadyToUpdate() {
        readyToUpdate = true;
    }
}
