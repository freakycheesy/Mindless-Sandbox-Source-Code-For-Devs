using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatItem : MonoBehaviourPunCallbacks
{
    public TMP_Text messenger_TMP;
    public TMP_Text message_TMP;

    [HideInInspector]
    public string[] nonoWords = { 
        "Fuck",
        "Bitch",
        "Cunt",
        "Prick",
        "Hoe",
        "Whore",
        "Bastard",
        "Cock",
        "Shit",
        "Dick",
        "Nigger",
        "Address",
        "Home",
        "State",
        "County",
        "Twat",
        "Number",
        "Ass",
        "Sex",
        "Porn",
    }; 

    [PunRPC]
    public void ReceiveMessageServerRPC(string messenger, string message) {
        transform.SetParent(FindAnyObjectByType<ChatScript>().contentTransform, false);
        messenger_TMP.text = messenger;
        message_TMP.text = message;
        for (int i = 0; i < nonoWords.Length; i++) {
            if (message_TMP.text.Contains(nonoWords[i], System.StringComparison.OrdinalIgnoreCase)) {
                PhotonNetwork.Destroy(gameObject.GetPhotonView());
            }
        }
    }
}
