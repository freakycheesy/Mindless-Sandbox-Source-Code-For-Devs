using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputActionProperty specialInput;
    [SerializeField] 
    InputActionProperty interactInput;
    [SerializeField] 
    Transform target;
    [SerializeField]
    GameObject interactionUI;
    public bool isSeated;
    PhotonView view;
    PlayerMovement playerMovement;
    Car car;

    private void Start() {
        isSeated = false;
        view = GetComponent<PhotonView>();
        playerMovement = GetComponent<PlayerMovement>();
        interactionUI = GameObject.FindGameObjectWithTag("interactionUI");
    }

    private void Update() {
        if (!view.IsMine) return;
        BasicInteractor();
        CarInteractor();
        InteractionUI();
    }

    void BasicInteractor() {
        if (Physics.Raycast(target.position, target.forward, out RaycastHit hit, 5f)) {
            PhotonView interactableView = hit.transform.gameObject.GetComponent<PhotonView>();
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            if (interactable != null) {
                if (interactInput.action.WasPressedThisFrame()) {
                    Debug.Log("WasPressedThisFrame");
                    interactableView.RequestOwnership();
                    interactableView.RPC("Pressed", RpcTarget.AllBuffered);
                }
                if (interactInput.action.WasPerformedThisFrame()) {
                    Debug.Log("WasPerformedThisFrame");
                    interactableView.RequestOwnership();
                    interactableView.RPC("Performed", RpcTarget.AllBuffered);
                }
                if (interactInput.action.WasReleasedThisFrame()) {
                    Debug.Log("WasReleasedThisFrame");
                    interactableView.RequestOwnership();
                    interactableView.RPC("Released", RpcTarget.AllBuffered);
                }
            }
        }
    }

    void CarInteractor() {     
        if (Physics.Raycast(target.position, target.forward, out RaycastHit hit, 5f) && interactInput.action.WasPerformedThisFrame()) {
            PhotonView interactableView = hit.transform.gameObject.GetComponent<PhotonView>();
            car = hit.transform.gameObject.GetComponent<Car>();
            if (car != null) {
                view.RPC("StopCrouch", RpcTarget.AllBufferedViaServer);
                Debug.Log("WasPerformedThisFrame");
                interactableView.RequestOwnership();
                interactableView.RPC("ToggleSeat", RpcTarget.AllBuffered);
                isSeated = car.isSeated;
            }
        }
    }

    void InteractionUI() {
        if (Physics.Raycast(target.position, target.forward, out RaycastHit hit, 5f)) {
            PhotonView interactableView = hit.transform.gameObject.GetComponent<PhotonView>();
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            Car carInteractable = hit.transform.gameObject.GetComponent<Car>();
            if (interactableView != null && (interactable != null || carInteractable != null)) {
                interactionUI.SetActive(true);
            }
            else {
                interactionUI.SetActive(false);
            }
        }
        else {
            interactionUI.SetActive(false);
        }
    }

    private void LateUpdate() {
        car.engineSound.enabled = isSeated;
        if (isSeated && car.view.IsMine && !GetComponent<WeaponTarget>().isDead) {
            transform.position = car.seat.position;
            GetComponent<Rigidbody>().position = car.seat.position;
            GetComponent<Rigidbody>().rotation = car.seat.rotation;
            playerMovement.x = 0;
            playerMovement.y = 0;
            car.horizontal = playerMovement.moveInput.action.ReadValue<Vector2>().x;
            car.vertical = playerMovement.moveInput.action.ReadValue<Vector2>().y;        
            if (playerMovement.crouching || playerMovement.jumping || GetComponent<WeaponTarget>().isDead) {
                car.GetComponent<PhotonView>().RPC("ToggleSeat", RpcTarget.AllBuffered);
                isSeated = false;
            }
        }
        else if(!isSeated || car.view.IsMine || GetComponent<WeaponTarget>().isDead) {
            transform.rotation = Quaternion.identity;
            car = null;
        }

        if(isSeated && transform.parent != car.seat) {
            photonView.RPC(nameof(ParentPlayerToCarSeat), RpcTarget.AllBufferedViaServer, true);
        }
        else if (!isSeated && transform.parent == car.seat) {
            photonView.RPC(nameof(ParentPlayerToCarSeat), RpcTarget.AllBufferedViaServer, false);
        }
    }

    [PunRPC]
    public void ParentPlayerToCarSeat(bool value) {
        if (value) {
            transform.SetParent(car.seat);
        }
        else {
            transform.SetParent(FindAnyObjectByType<PlayerList>().transform);
        }
    }

}
