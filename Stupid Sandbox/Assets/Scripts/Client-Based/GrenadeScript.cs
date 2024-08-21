using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GrenadeScript : MonoBehaviourPunCallbacks
{
    public float delay = 3f;
    public float radius = 6f;
    public float force = 500f;
    public float damage = 50f;

    public GameObject explosionEffect;

    float countdown;
    bool hasExploded = false;
    Animator scoreAnimator;
    TMP_Text scoreText;

    // Start is called before the first frame update
    private void Start() {
        if (!photonView.IsMine) return;
        countdown  = delay;
        scoreAnimator = GameObject.FindGameObjectWithTag("scoreUI").GetComponent<Animator>();
        scoreText = GameObject.FindGameObjectWithTag("scoreUI").GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!photonView.IsMine)
            return;
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded) {
            photonView.RPC(nameof(Explode), RpcTarget.AllBuffered);
            hasExploded = true;
        }
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
            if(weaponTarget != null) {
                weaponTarget.TakeDamage(damage);
                if (weaponTarget.photonView.IsMine && weaponTarget.gameObject.name.Contains("Player")) {
                    Debug.Log("Friendly Fire");
                }
                else {
                    if (weaponTarget.health > 0) {
                        photonView.RPC("GetPoints", RpcTarget.AllBuffered, damage);
                        scoreText.text = "+" + damage.ToString();
                        scoreAnimator.ResetTrigger("Score");
                        scoreAnimator.Play("Idle", 0);
                        scoreAnimator.SetTrigger("Score");
                    }
                    else {
                        scoreText.text = "Target Down!";
                        scoreAnimator.ResetTrigger("Score");
                        scoreAnimator.Play("Idle", 0);
                        scoreAnimator.SetTrigger("Score");
                    }
                }              
            }
            PhotonNetwork.Destroy(gameObject);
        }       
    }
}
