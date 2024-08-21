using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindlessMods {
    public class ModPhysicsSimulation : MonoBehaviour {
        public float gravityY = -30f;

        public bool stopPhysicsSimulation = true;
        public bool reverseGravity = false;
        public Rigidbody[] rigidbodies;

        private void Start() {
            stopPhysicsSimulation = true;
            reverseGravity = false;
            rigidbodies = FindObjectsOfType<Rigidbody>();
            Physics.gravity = new Vector3(0, 0, 0);
        }

        private void Update() {
            rigidbodies = FindObjectsOfType<Rigidbody>();
            if (stopPhysicsSimulation) {
                Physics.gravity = new Vector3(0, 0, 0);
                foreach (Rigidbody rb in rigidbodies) {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }

            else {
                Physics.gravity = new Vector3(0, gravityY, 0);
            }

            if (reverseGravity) {
                Physics.gravity = new Vector3(0, -gravityY, 0);
            }
            else {
                Physics.gravity = new Vector3(0, gravityY, 0);
            }
        }

        public void TogglePhysicsSimulation(bool idx) {
            stopPhysicsSimulation = idx;         
        }

        public void CounterCurrentGravity(bool idx) {
            reverseGravity = idx;
        }
    }
}