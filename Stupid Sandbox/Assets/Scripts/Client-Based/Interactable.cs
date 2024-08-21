using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviourPunCallbacks {
    // Serialized
    [SerializeField]
    UnityEvent onPressedEvent;
    [SerializeField]
    UnityEvent onPerformedEvent;
    [SerializeField]
    UnityEvent onReleasedEvent;
    [SerializeField]
    UnityEvent ToggleOn;
    [SerializeField]
    UnityEvent ToggleOff;
    [SerializeField]
    bool isToggleInteractable;
    [SerializeField]
    public bool isToggled = false;
    // Not Serialized
    PhotonView view;

    void Start () {
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Pressed() {
        view.RequestOwnership();
        onPressedEvent.Invoke();
    }
    [PunRPC]
    public void Performed() {
        view.RequestOwnership();
        onPerformedEvent.Invoke();
        if (isToggleInteractable) {
            view.RPC("Toggle", RpcTarget.AllBufferedViaServer);
        }
    }
    [PunRPC]
    public void Released() {
        view.RequestOwnership();
        onReleasedEvent.Invoke();
    }

    [PunRPC]
    public void Toggle() {
        view.RequestOwnership();
        if(view.IsMine) {
            isToggled = !isToggled;
        }
        if (isToggled) {
            ToggleOn.Invoke();
        }
        else {
            ToggleOff.Invoke();
        }
    }
}
