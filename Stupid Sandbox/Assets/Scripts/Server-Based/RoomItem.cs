using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomHost;
    public TMP_Text roomName;
    public TMP_Text roomPlayers;
    public TMP_Text roomMap;
    public string code;
    MatchmakeRooms manager;
    private void Start() {
        manager = FindObjectOfType<MatchmakeRooms>();
    }
    public void SetRoomHost(string _roomHost) {
        roomHost.text = "Hosted by " + _roomHost;
    }
    public void SetRoomName(string _roomName) {
        roomName.text = _roomName;
    }
    public void SetRoomPlayers(string _roomPlayers) {
        roomPlayers.text = _roomPlayers;
    }
    public void SetRoomMap(string _roomMap) {
        roomMap.text = _roomMap;
    }
    public void SetCode(string _code) {
        code = _code;
    }
    public void OnClickItem() {
        manager.JoinCustomRoom(code);
    }
}
