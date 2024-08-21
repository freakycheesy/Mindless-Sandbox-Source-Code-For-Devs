using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNewPosition : MonoBehaviourPunCallbacks
{
    public Vector3 NewPosition;

    public Vector3 OldPosition;

    public float followRate;

    Vector3 selectedPos;

    private void Start() {
        selectedPos = transform.localPosition;
    }

    public void GoToOldPos() {
        selectedPos = OldPosition;
    }

    public void GoToNewPos() {
        selectedPos = NewPosition;
    }

    private void LateUpdate() {
        transform.localPosition = Vector3.Lerp(transform.localPosition, selectedPos, followRate * Time.deltaTime);
    }
}
