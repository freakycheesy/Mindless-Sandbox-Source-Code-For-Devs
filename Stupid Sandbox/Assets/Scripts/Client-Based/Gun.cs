using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Animator holderAnimator;
    [Header("Input")]
    public InputActionProperty shootInput;
    public InputActionProperty aimInput;
    public InputActionProperty reloadInput;
    [Header("Values")]
    public float damage = 10f;
    float tempDamage;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 15f;
    public float bulletSpreadAmount = 0.015f;
    float tempSpread;
    public int bulletsShot = 1;
    float tempSpeed;
    public Camera playerCamera;
    float tempfov;
    public GameObject muzzleFlash;
    public GameObject impactEffect;
    public Transform bulletSpawn;
    public PhotonView view;
    public Transform target;
    public Transform myWeaponHolder;
    float lerpWeaponHolder;
    Animator animator;
    public float fovMultiplier = 1.5f;
    public float spreadDivision = 2;
    public bool SetInsteadOfDivide = false;
    public float animationSpeed = 1;
    private float nextTimeToFire = 0f;
    public Pause pause;
    public SpawnMenu spawnMenu;
    [SerializeField] bool parentMuzzleFlash = true;
    public bool soundOnImpact;
    private bool auto = true;
    AudioSource impactAudioSource;
    bool aiming;
    Animator scoreAnimator;
    TMP_Text scoreText;
    TMP_Text ammoText;

    // Ammo
    public int maxAmmo = 15;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool reloading = false;


    private void OnDisable() {
        playerCamera.fieldOfView = tempfov;
        damage = tempDamage;
        bulletSpreadAmount = tempSpread;
        aiming = false;
    }
    private void OnEnable() {
        reloading = false;
    }
    private void Awake() {
        animator = GetComponent<Animator>();
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
        scoreAnimator = GameObject.FindGameObjectWithTag("scoreUI").GetComponent<Animator>();
        scoreText = GameObject.FindGameObjectWithTag("scoreUI").GetComponentInChildren<TMP_Text>();

        impactAudioSource = GetComponent<AudioSource>();
        tempSpread = bulletSpreadAmount;
        tempfov = 90;
        tempDamage = damage;
        aiming = false;
        if (animator != null) {
            animator.speed = animationSpeed;
        }
    }
    private void Start() {
        ammoText = GameObject.FindGameObjectWithTag("Ammo").GetComponent<TMP_Text>();
        currentAmmo = maxAmmo;
    }
    private void LateUpdate() {
        holderAnimator.SetBool("Reload", reloading);
    }
    void Update() {
        if (!view.IsMine || pause.isPaused || (spawnMenu != null && spawnMenu.isMenuOpened)) {
            playerCamera.fieldOfView = tempfov;
            damage = tempDamage;
            bulletSpreadAmount = tempSpread;
            aiming = false;
            return; 
        }
        if (aimInput != null) {
            if (aimInput.action.WasPressedThisFrame()) {
                aiming = !aiming;
            }
            if (aiming) {
                playerCamera.fieldOfView = tempfov / fovMultiplier;
                damage = tempDamage + 1.5f;
                if (SetInsteadOfDivide) {
                    bulletSpreadAmount = spreadDivision;
                }
                else {
                    bulletSpreadAmount = tempSpread / spreadDivision;
                }
            }
            else {
                playerCamera.fieldOfView = tempfov;
                damage = tempDamage;
                bulletSpreadAmount = tempSpread;
            }
        }
        if (reloading) {
            return;
        }
        if (currentAmmo <= 0 || (reloadInput.action.WasPressedThisFrame() && currentAmmo < maxAmmo)) {
            StartCoroutine(Reload());
            return;
        }
        if (auto) {
            if (shootInput.action.IsPressed() && Time.time >= nextTimeToFire) {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else {
            if (shootInput.action.WasPressedThisFrame() && Time.time >= nextTimeToFire) {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }     
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    IEnumerator Reload() {
        Debug.Log("Reloading..");
        reloading = true;
        ammoText.text = "Reloading...";
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        currentAmmo = maxAmmo;
    }

    private void Shoot() {
        if (animator != null) {
            animator.ResetTrigger("Shoot");
        }
        if (muzzleFlash != null) {
            GameObject muzzleFlashGO = PhotonNetwork.Instantiate(muzzleFlash.name, bulletSpawn.position, bulletSpawn.rotation);
            if (parentMuzzleFlash) {
                muzzleFlashGO.transform.parent = bulletSpawn.transform;
            }
        }
        if (animator != null) {
            animator.SetTrigger("Shoot");
        }
        currentAmmo--;
        for (int x = 0; x < bulletsShot; x++) {
            RaycastHit hit;
            Vector3 fwd = target.transform.forward;
            fwd = fwd + target.TransformDirection(new Vector3(UnityEngine.Random.Range(-bulletSpreadAmount, bulletSpreadAmount), UnityEngine.Random.Range(-bulletSpreadAmount, bulletSpreadAmount)));
            if (Physics.Raycast(target.position, fwd, out hit, range)) {
                if (hit.collider != null) {
                    Debug.LogError(hit.transform.name);
                    PhotonNetwork.Instantiate(impactEffect.name, hit.point, Quaternion.LookRotation(hit.normal));
                    WeaponTarget weaponTarget = hit.transform.GetComponent<WeaponTarget>();
                    if(impactAudioSource != null) {
                        if (soundOnImpact) {
                            impactAudioSource.Play();
                        }
                    }                    
                    if (hit.rigidbody != null) {
                        hit.rigidbody.AddForce(-hit.normal * impactForce);
                    }
                    if (weaponTarget != null && damage != 0) {
                        weaponTarget.TakeDamage(damage);
                        if (weaponTarget.photonView.IsMine && weaponTarget.gameObject.name.Contains("Player")) {
                            Debug.Log("Friendly Fire");
                        }
                        else {
                            if (weaponTarget.health > 0) {
                                view.RPC("GetPoints", RpcTarget.AllBufferedViaServer, damage);
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
                }
            }           
        }
        view.GetComponent<Rigidbody>().AddForce(-target.forward * impactForce * 1.5f);
    }

}
