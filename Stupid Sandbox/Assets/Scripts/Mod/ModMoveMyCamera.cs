using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MindlessMods {
    public class ModMoveMyCamera : MonoBehaviour {

        float xRotation = 0f;
        float yRotation = 0f;
        Vector2 moveVector;
        Vector3 moveVector3;
        public float moveSpeed = 1;
        public float rotateSpeed = 1;
        bool isCursorLocked;

        public InputActionProperty moveInput;
        public InputActionProperty lookInput;

        public void MoveCamera() {
            if (moveInput.action.WasReleasedThisFrame())
                moveVector3 = new Vector3(0, 0, 0);
            if (!isCursorLocked)
                return;
            if (moveInput.action.WasPerformedThisFrame()) {
                moveVector = moveInput.action.ReadValue<Vector2>();
                moveVector3 = new Vector3(moveVector.x, 0, moveVector.y);
            }
        }

        public void LookCamera() {
            Vector2 lookVector;
            if (!isCursorLocked) {
                lookVector = new Vector3(0, 0, 0);
                return;
            }
            lookVector = lookInput.action.ReadValue<Vector2>();
            xRotation -= lookVector.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation += lookVector.x;
        }

        private void Update() {
            transform.localPosition += transform.localRotation * moveVector3 * moveSpeed;
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

            isCursorLocked = Mouse.current.rightButton.IsPressed();

            if (isCursorLocked) {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else {
                Cursor.lockState = CursorLockMode.None;
            }

            MoveCamera(); LookCamera();
        }
    }
}

