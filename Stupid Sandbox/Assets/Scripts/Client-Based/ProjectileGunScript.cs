using Photon.Pun;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileGunScript : MonoBehaviour
{
    public Animator holderAnimator;
    [Header("Input")]
    public InputActionProperty shootInput;
    public InputActionProperty aimInput;
    public InputActionProperty reloadInput;
    [Header("Values")]
    public float damage = 10f;
    public float impactForce = 20f;
    public float forwardForce = 20, verticalForce = 0;
    float tempDamage;
    public float fireRate = 15f;
    public int bulletsShot = 1;
    float tempSpeed;
    public Camera playerCamera;
    float tempfov;
    public GameObject muzzleFlash;
    public Transform bulletSpawn;
    public PhotonView view;
    public PhotonView projectileObject;
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
    private bool auto = true;
    AudioSource impactAudioSource;
    bool aiming;
    TMP_Text ammoText;

    // Ammo
    public int maxAmmo = 15;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool reloading = false;

    private void OnDisable() {
        playerCamera.fieldOfView = tempfov;
        damage = tempDamage;
        aiming = false;
    }
    private void OnEnable() {
        reloading = false;
    }
    private void Awake() {
        animator = GetComponent<Animator>();
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
        impactAudioSource = GetComponent<AudioSource>();
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
            }
            else {
                playerCamera.fieldOfView = tempfov;
                damage = tempDamage;
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
            GameObject projectile = PhotonNetwork.Instantiate(projectileObject.name, bulletSpawn.position, bulletSpawn.rotation);
            projectile.GetPhotonView().RequestOwnership();
            projectile.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * forwardForce, ForceMode.Impulse);
            projectile.GetComponent<Rigidbody>().AddForce(Vector3.up * verticalForce, ForceMode.Impulse);
        }

        view.GetComponent<Rigidbody>().AddForce(-target.forward * impactForce * 1.5f);
    }
}
