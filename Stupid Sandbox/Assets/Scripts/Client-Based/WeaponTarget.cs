using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponTarget : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health = 50f;
    float maxHealth;
    [SerializeField]
    GameObject deathParticle;
    [SerializeField]
    GameObject deathObject;
    [SerializeField]
    UnityEvent DamageEvent;
    [SerializeField]
    UnityEvent DeathEvent;
    [SerializeField]
    UnityEvent RespawnEvent;
    [SerializeField]
    PhotonView view;
    [SerializeField]
    Animator damageVignette;
    [SerializeField]
    Slider healthBar;
    [SerializeField]
    bool isPlayer = false;
    [SerializeField]
    bool canRespawn = false;
    [SerializeField]
    Transform[] defaultSpawnPoint;
    [SerializeField]
    Button respawnBtn;

    public bool isDead;

    private void Awake() {
        maxHealth = health;
        view = GetComponent<PhotonView>();
        isDead = false;
        if(!view.IsMine && !isPlayer) return;
            damageVignette = GameObject.FindGameObjectWithTag("DamageVignette").GetComponent<Animator>();
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            defaultSpawnPoint = GameObject.FindAnyObjectByType<NetworkManager>().spawnPoints;
    }

    private void Update() {
        if (view.IsMine && (isDead || health <= 0) && canRespawn) {           
            view.RPC(nameof(RespawnRPC), RpcTarget.AllBufferedViaServer);
        }
        if (respawnBtn == null) {
            respawnBtn = GameObject.FindGameObjectWithTag("respawnBtn").GetComponent<Button>();
            respawnBtn.onClick.AddListener(() => {
                if (view.IsMine && canRespawn) {
                    view.RPC("SetDeathCounter", RpcTarget.AllBufferedViaServer);
                    isDead = true;
                    view.RPC(nameof(RespawnRPC), RpcTarget.AllBufferedViaServer);
                }
            });
        }
        if(health > maxHealth) {
            health = maxHealth;
        }
        else if (health < 0) {
            health = 0;
        }
    }
    [PunRPC]
    public void TeleportObject() {
        int x = Random.Range(0, defaultSpawnPoint.Length);
        GetComponent<Rigidbody>().position = defaultSpawnPoint[x].position;
        GetComponent<Rigidbody>().rotation = defaultSpawnPoint[x].rotation;
    }
    void RespawnTimer() {
        view.RPC(nameof(RespawnRPC), RpcTarget.AllBufferedViaServer);
    }
    [PunRPC]
    void RespawnRPC() {     
        isDead = false;
        health = maxHealth;
        Interactor interactor = GetComponent<Interactor>();
        if(interactor != null && interactor.isSeated) {
            interactor.isSeated = false;
        }        
        view.RPC(nameof(TeleportObject), RpcTarget.AllBuffered);
        health = maxHealth;
        TakeDamage(0);
        RespawnEvent.Invoke();
        isDead = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if(Mathf.Abs(collision.rigidbody.velocity.magnitude) > 5) {
            if (collision.gameObject.GetComponent<Car>() != null || collision.gameObject.GetComponent<WeaponTarget>().isPlayer) return;
            if (collision.rigidbody.mass >= 1) {
                TakeDamage(Mathf.Abs(collision.rigidbody.mass) / 2.5f);        
            }
        }
    }

    public void TakeDamage(float amount) {
        view.RPC("RPC_TakeDamage", RpcTarget.AllViaServer, amount);
        if(isPlayer && view.IsMine)
            healthBar.value = health;
        health -= amount;
        DamageEvent.Invoke();
        if (health <= 0 && !isDead) {
            isDead = true;
            health = 0;
            DeathEvent.Invoke();
            if (isPlayer) {
                view.RPC("SetDeathCounter", RpcTarget.AllBufferedViaServer);
            }
        }
    }

    [PunRPC]
    void RPC_TakeDamage(float damage) {
        if (!view.IsMine) return;
        health -= damage;
        DamageEvent.Invoke();
        if (health <= 0) {
            health = 0;
            DeathEvent.Invoke();
        }
        if (damage >= 0) {
            // Put Something Here Later
        }
        if (deathParticle != null) {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, Quaternion.identity);
        }
        if (isPlayer) {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(-Vector3.forward * damage);
            if (damage <= 0) {
                damageVignette.GetComponent<Image>().color = new Color(0, 1, 0);
            }
            else {
                damageVignette.GetComponent<Image>().color = new Color(1, 0, 0);
            }
            damageVignette.ResetTrigger("Damage");
            damageVignette.SetTrigger("Damage");
            healthBar.value = health;
        }
    }

    public void Delete() {
        if (deathParticle != null) {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, transform.rotation);
        }
        if (deathObject != null) {
            PhotonNetwork.Instantiate(deathObject.name, transform.position, transform.rotation);
        }
        PhotonNetwork.Destroy(gameObject);
    }

    public void SpawnDeathObject() {
        if (deathParticle != null) {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, transform.rotation);
        }
        if (deathObject != null) {
            PhotonNetwork.Instantiate(deathObject.name, transform.position, transform.rotation);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(health);
        }
        else {
            health = (float)stream.ReceiveNext();
        }
    }
}
