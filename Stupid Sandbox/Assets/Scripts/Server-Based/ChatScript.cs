using UnityEngine;
using Photon.Pun;

public class ChatScript : MonoBehaviourPun
{
    public ChatItem chatItem;
    public Transform contentTransform;
    public bool canMessage = true;

    public void Start() {
        canMessage = true;
    }

    public void SendMessageToServer(string message) {
        if (!canMessage && message == "") return;
        canMessage = false;
        for (int i = 0; i < chatItem.nonoWords.Length; i++) {
            if (message.Contains(chatItem.nonoWords[i], System.StringComparison.OrdinalIgnoreCase)) {
                return;
            }
        }
        GameObject chatItemObject = PhotonNetwork.Instantiate(chatItem.name, Vector3.zero, Quaternion.identity);
        chatItemObject.GetPhotonView().RPC(nameof(chatItem.ReceiveMessageServerRPC), RpcTarget.AllBuffered, PhotonNetwork.NickName, message);
        Invoke(nameof(Cooldown), 5f);
    }

    void Cooldown() {
        canMessage = true;
    }
}
