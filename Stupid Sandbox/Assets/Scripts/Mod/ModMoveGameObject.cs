using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MindlessMods {
    public class ModMoveGameObject : MonoBehaviour {

        public Transform positionHolder;
        public GameObject selectedGameObject;
        public ModMoveMyCamera modMoveMyCamera;
        public bool isMoving;

        private void LateUpdate() {
            RaycastHit hit;
            bool hasHitGameObject = Physics.Raycast(transform.position, transform.forward, out hit, 5f);
            if ((hasHitGameObject && hit.collider.GetComponent<Grabbable>()) && (Mouse.current.leftButton.wasPressedThisFrame && Mouse.current.rightButton.IsPressed())) {
                isMoving = !isMoving;
                if (isMoving) {
                    selectedGameObject = hit.collider.gameObject;
                }
                else {
                    selectedGameObject = null;
                }
            }
            if (isMoving) {
                modMoveMyCamera.moveSpeed = 0.005f;
                selectedGameObject.transform.position = positionHolder.position;
                if (selectedGameObject.GetComponent<Rigidbody>() != null) {
                    selectedGameObject.GetComponent<Rigidbody>().position = positionHolder.position;
                }
            }
            else {
                modMoveMyCamera.moveSpeed = 0.01f;
            }
        }
    }
}