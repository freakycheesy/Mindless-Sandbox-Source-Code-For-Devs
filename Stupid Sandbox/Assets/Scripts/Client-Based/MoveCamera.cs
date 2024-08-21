using Photon.Pun;
using System.Globalization;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform player;
    public Transform oldHead;
    public Transform head;
    void Update() {
        transform.position = player.transform.position;
        head.transform.rotation = transform.rotation;
        oldHead.transform.rotation = transform.rotation;
    }
}
