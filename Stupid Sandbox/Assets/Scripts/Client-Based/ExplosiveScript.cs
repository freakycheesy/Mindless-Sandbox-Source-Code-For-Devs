using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExplodeScript : MonoBehaviourPunCallbacks 
{
    public float delay = 3f;
    public float radius = 6f;
    public float force = 500f;
    public float damage = 50f;

    public GameObject explosionEffect;

    float countdown;
    bool detonate = false;
    bool hasExploded = false;

    // Start is called before the first frame update
    private void Start() {
        if (!photonView.IsMine)
            return;
        countdown = delay;
    }

    // Update is called once per frame
    private void Update() {
        if (!photonView.IsMine || !detonate)
            return;
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded) {
            photonView.RPC(nameof(Explode), RpcTarget.AllBuffered);
            hasExploded = true;
        }
    }

    public void DetonateEvent() {
        detonate = true;
    }

    [PunRPC]
    public void Explode() {
        GameObject explosion = PhotonNetwork.Instantiate(explosionEffect.name, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders) {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            WeaponTarget weaponTarget = nearbyObject.GetComponent<WeaponTarget>();
            if (weaponTarget != null) {
                weaponTarget.TakeDamage(damage);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision == null) return;
        if(GetComponent<Rigidbody>().velocity.magnitude > 10) {
            DetonateEvent();
        }
    }
}
