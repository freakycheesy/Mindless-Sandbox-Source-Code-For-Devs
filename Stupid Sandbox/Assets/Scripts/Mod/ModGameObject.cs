using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindlessMods {
    public class ModGameObject : MonoBehaviour {

        private void Awake() {
            if(FindObjectOfType<ModSaveSystem>() == null) {
                Destroy(this);
            }
        }

        private void OnDestroy() {
            ModSaveSystem.objects.Remove(this);
        }
        void Start () {
            ModSaveSystem.objects.Add(this);
        }
    }
}

