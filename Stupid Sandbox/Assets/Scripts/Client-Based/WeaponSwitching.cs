using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    public InputActionProperty switchInput;
    public PhotonView view;
    public Pause pause;
    public SpawnMenu spawnMenu;
    public Transform weaponsPivot;
    public float delay;

    bool canSwitch;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
        if (weaponsPivot == null) {
            weaponsPivot = GetComponent<Transform>();
        }
        yield return new WaitForSeconds(delay);
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine || pause.isPaused || (spawnMenu != null && spawnMenu.isMenuOpened)) return;
        float switchValue = switchInput.action.ReadValue<float>();
        if(switchValue >= 0.5f && switchInput.action.WasPerformedThisFrame()) {
            if (!canSwitch) return;
            if(selectedWeapon >= weaponsPivot.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;            
            SelectWeapon();
        }
        if (switchValue <= -0.5f && switchInput.action.WasPerformedThisFrame()) {
            if (!canSwitch) return;
            if (selectedWeapon <= 0)
                selectedWeapon = weaponsPivot.childCount - 1;
            else
                selectedWeapon--;
            SelectWeapon();
        }


        // Number Keys
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            if (!canSwitch) return;
            selectedWeapon = 0;
            SelectWeapon();
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && weaponsPivot.childCount >= 2) {
            if (!canSwitch) return;
            selectedWeapon = 1;
            SelectWeapon();
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && weaponsPivot.childCount >= 3) {
            if (!canSwitch) return;
            selectedWeapon = 2;
            SelectWeapon();
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && weaponsPivot.childCount >= 4) {
            if (!canSwitch) return;
            selectedWeapon = 3;
            SelectWeapon();
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && weaponsPivot.childCount >= 5) {
            if (!canSwitch) return;
            selectedWeapon = 4;
            SelectWeapon();
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && weaponsPivot.childCount >= 6) {
            if (!canSwitch) return;
            selectedWeapon = 5;
            SelectWeapon();
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && weaponsPivot.childCount >= 7) {
            if (!canSwitch) return;
            selectedWeapon = 6;
            SelectWeapon();
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && weaponsPivot.childCount >= 8) {
            if (!canSwitch) return;
            selectedWeapon = 7;
            SelectWeapon();
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && weaponsPivot.childCount >= 9) {
            if (!canSwitch) return;
            selectedWeapon = 8;
            SelectWeapon();
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame && weaponsPivot.childCount >= 10) {
            if (!canSwitch) return;
            selectedWeapon = 9;
            SelectWeapon();
        }
    }

    void SelectWeapon() {
        canSwitch = false;
        Invoke(nameof(ResetCooldown), delay);
        view.RPC("SelectWeapon", RpcTarget.AllBufferedViaServer, selectedWeapon);
        int i = 0;
        foreach (Transform weapon in weaponsPivot) {
            if (i == selectedWeapon) {
                weapon.gameObject.SetActive(true);
            }
            else {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    void ResetCooldown() {
        canSwitch = true;
    }
}
