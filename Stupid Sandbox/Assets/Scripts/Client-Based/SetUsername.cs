using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetUsername : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;
    private void Awake() {
        if (PlayerPrefs.GetString("username") == "") {
            PlayerPrefs.SetString("username", "PLAYER" + Random.Range(0, 9999));
        }
        inputField = GetComponent<TMP_InputField>();
        inputField.text = PlayerPrefs.GetString("username");
    }
    public void ChangeUsername(string username) {
        username = username.ToUpper();
        if (username == "") {
            inputField.text = PlayerPrefs.GetString("username");
            return;
        }
        PlayerPrefs.SetString("username", username);
        PhotonNetwork.NickName = PlayerPrefs.GetString("username") + Random.Range(0, 9999);
    }
}
