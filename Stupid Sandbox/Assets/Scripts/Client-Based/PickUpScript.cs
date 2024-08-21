using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviourPunCallbacks
{
    public Transform pickUpTransform;
    public float rotateSpeed;
    public string colliderTag;
    public float[] amounts;

    private float rotateFloatValue = 0;
    private Vector3 rotateVectorValue = Vector3.zero;

    private void Update()
    {
        rotateFloatValue += rotateSpeed * Time.deltaTime;
        rotateVectorValue = new Vector3(0, rotateFloatValue, 0);
        pickUpTransform.eulerAngles = rotateVectorValue;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == colliderTag) {
            if (colliderTag == "Player") {
                other.GetComponent<WeaponTarget>().TakeDamage(amounts[0]);
            }
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
