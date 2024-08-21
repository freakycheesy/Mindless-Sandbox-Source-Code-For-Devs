using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
public class IsTriggerEvent : MonoBehaviour
{
    [SerializeField]
    bool isDeadlyToPlayers;
    [SerializeField]
    float damage = 100;
    [SerializeField]
    string TriggereTag;
    [SerializeField]
    UnityEvent TriggerEnterEvent;
    [SerializeField]
    UnityEvent TriggerExitEvent;

    public float fireRate = 2.5f;

    private float nextTimeToFire = 0f;
    Collider otherCollider;
    Collision otherCollision;

    private void OnTriggerEnter(Collider other) {
        otherCollider = other;
        if (otherCollider.tag == TriggereTag) {
            TriggerEnterEvent.Invoke();       
        }
    }

    void OnTriggerExit(Collider other) {
        otherCollider = null;
        if (otherCollider.tag == TriggereTag) {
            TriggerExitEvent.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        otherCollision = collision;
        if(collision.gameObject.tag == TriggereTag) {
            TriggerEnterEvent.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision) {
        otherCollision = null;
        if (collision.gameObject.tag == TriggereTag) {
            TriggerExitEvent.Invoke();
        }
    }

    private void Update() {
        if (isDeadlyToPlayers && otherCollider != null) {
            if (otherCollider.GetComponent<PhotonView>() != null && (otherCollider.tag == TriggereTag) && Time.time >= nextTimeToFire) {
                nextTimeToFire = Time.time + 1f / fireRate;
                otherCollider.GetComponent<WeaponTarget>().TakeDamage(damage);
            }
        }
    }
}
