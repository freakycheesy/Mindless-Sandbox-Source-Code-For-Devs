using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.ProBuilder.Shapes;

public class MatchmakeRooms : MonoBehaviourPunCallbacks {

    string matchmakeInput;
    bool privateToggle = false;
    public bool offlineMode = false;
    public string selectedMap;
    public RoomItem roomItemObject;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;
    public GameObject ConnectingMessage;
    public Slider maxPlayerSlider;
    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public override void OnDisable() {
        if(ConnectingMessage != null) {
            ConnectingMessage.SetActive(true);
        }
    }

    public void updateMap(string map) {
        selectedMap = map;
    }

    public void MatchmakeRoom() {
        if (selectedMap == "") return;
        PhotonNetwork.EnableCloseConnection = true;
        RoomOptions roomOptions = new RoomOptions();
        ExitGames.Client.Photon.Hashtable RoomCustomProps = new ExitGames.Client.Photon.Hashtable();
        RoomCustomProps.Add("h", PlayerPrefs.GetString("username"));
        RoomCustomProps.Add("n", PlayerPrefs.GetString("username") + "'s Game");
        RoomCustomProps.Add("m", selectedMap);
        if(maxPlayerSlider != null)
            roomOptions.MaxPlayers = Mathf.RoundToInt(maxPlayerSlider.value);
        else
            roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = !privateToggle;
        roomOptions.CustomRoomProperties = RoomCustomProps;
        roomOptions.CustomRoomPropertiesForLobby = new string[3] { "h", "n", "m"};
        TryCreateRoom(roomOptions);
        
    }

    private void TryCreateRoom(RoomOptions roomOptions) {
        try {
            PhotonNetwork.CreateRoom(Random.Range(100000, 999999).ToString(), roomOptions, TypedLobby.Default);
        }
        catch {
            TryCreateRoom(roomOptions);
        }
    }

    public void JoinRoom() {
        if (matchmakeInput != "") {
            PhotonNetwork.JoinRoom(matchmakeInput);
        }
        else {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log(cause);
        DisconnectToServer();
    }
    public void DisconnectToServer() {
        if (contentObject != null) {
            foreach (Transform t in contentObject) {
                Destroy(t.gameObject);
            }
        }
        PhotonNetwork.Disconnect();
    }
    public void changeMatchmakeInput(string input) {
        matchmakeInput = input;
    }

    public void changePrivateToggle(bool toggle) {
        privateToggle = toggle;
    }
    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.CustomProperties["m"].ToString());
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        if (contentObject == null)
            return;
        if (Time.time >= nextUpdateTime) {
            UpdateRoomList(roomList);
            for(int i = 0; i < roomList.Count; i++) {
                Debug.LogError(roomList[i]);
            }
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }
    private void UpdateRoomList(List<RoomInfo> list) {
        foreach(RoomItem item in roomItemsList) {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach(RoomInfo room in list) {
            RoomItem newRoom = Instantiate(roomItemObject, contentObject);
            newRoom.SetRoomHost(room.CustomProperties["h"].ToString());
            newRoom.SetRoomName(room.CustomProperties["n"].ToString());
            newRoom.SetRoomPlayers((room.PlayerCount + "/" + room.MaxPlayers).ToString());
            newRoom.SetRoomMap(room.CustomProperties["m"].ToString());
            newRoom.SetCode(room.Name);
            roomItemsList.Add(newRoom);
        }

    }
    public void JoinCustomRoom(string roomCode) {
        PhotonNetwork.JoinRoom(roomCode);
        if (ConnectingMessage != null) {
            ConnectingMessage.SetActive(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        if (ConnectingMessage != null) {
            ConnectingMessage.SetActive(false);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        if (ConnectingMessage != null) {
            ConnectingMessage.SetActive(false);
        }
    }

    public override void OnConnectedToMaster() {
        if (ConnectingMessage != null) {
            ConnectingMessage.SetActive(false);
        }
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        Debug.LogError(PhotonNetwork.CloudRegion);
    }

    private void LateUpdate() {
        PhotonNetwork.OfflineMode = offlineMode;
    }
}
