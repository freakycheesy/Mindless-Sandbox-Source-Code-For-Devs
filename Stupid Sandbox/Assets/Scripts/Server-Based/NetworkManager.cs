using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Transform playerList;

    public GameObject playerPrefab;

    public GameObject[] disableInOfflineMode;

    public Transform defaultSpawnPoint;

    public Transform[] spawnPoints;

    public string currentRoomCode;

    void Start() {
        Cursor.lockState = CursorLockMode.None;

        defaultSpawnPoint = GameObject.FindGameObjectWithTag("DefaultSpawn").GetComponent<Transform>();
        spawnPoints = defaultSpawnPoint.gameObject.GetComponentsInChildren<Transform>();

        //Debug.Log(PhotonNetwork.CurrentRoom.Name.ToString());
    }

    public void CopyRoomCodeToClipboard() {
        TextEditor textEditor = new TextEditor();
        textEditor.text = currentRoomCode;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public void SpawnPlayer() {
        GameManager gameManager = GetComponent<GameManager>();
        if (!gameManager.isStarted) return;
        int spawnPoint = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[spawnPoint].position;
        PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPosition, Quaternion.Euler(0, spawnPoints[spawnPoint].rotation.y, 0));
        Cursor.lockState = CursorLockMode.Locked;
        if (PhotonNetwork.OfflineMode) {
            foreach (GameObject gameObject in disableInOfflineMode) {
                gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if (!PhotonNetwork.IsConnected)
            return;
        NetworkPlayer[] playerObjects = FindObjectsOfType<NetworkPlayer>();
        if (currentRoomCode == "") {
            currentRoomCode = PhotonNetwork.CurrentRoom.Name.ToString();
        }
        for (int i = 0; i < playerObjects.Length; i++) {
            playerObjects[i].transform.SetParent(playerList);
        }
    }

    public void Disconnect() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        base.OnDisconnected(cause);
        Debug.Log(cause);
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer.IsMasterClient) {
            Disconnect();
        }
    }

}
