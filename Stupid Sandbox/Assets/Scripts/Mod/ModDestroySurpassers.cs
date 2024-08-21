using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindlessMods {
    public class ModDestroySurpassers : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (other == null)
                return;
            Destroy(other.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            if (other == null)
                return;
            Destroy(other.gameObject);
        }

        private void OnTriggerStay(Collider other) {
            if (other == null)
                return;
            Destroy(other.gameObject);
        }
    }
}

