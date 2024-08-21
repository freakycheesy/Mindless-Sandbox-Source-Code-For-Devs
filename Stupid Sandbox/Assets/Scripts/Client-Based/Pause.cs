using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button backToGameBtn;
    [SerializeField] public bool isPaused;
    [SerializeField] private InputActionProperty pauseInput;
    [SerializeField] private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        backToGameBtn.onClick.AddListener(() => {
            ToggleMenu(true);
        });
        menu.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        menu.SetActive(isPaused);
        if(pauseInput.action.WasPressedThisFrame()) {
            ToggleMenu(false);
        }
    }

    public void ToggleMenu(bool backToGame) {
        if(backToGame) {
            isPaused = false;
        }
        else {
            isPaused = !isPaused;
        }
        if (isPaused) {
            eventSystem.SetSelectedGameObject(backToGameBtn.gameObject);
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (eventSystem == null) {
            eventSystem = FindAnyObjectByType<EventSystem>();
        }
    }
}
