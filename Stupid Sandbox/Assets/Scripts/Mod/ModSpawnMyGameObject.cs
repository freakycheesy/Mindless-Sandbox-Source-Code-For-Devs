using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MindlessMods {
    public class ModSpawnMyGameObject : MonoBehaviour {

        public Transform spawnPostion;
        public Transform objectsHolder;

        public string extraText;
        public TMP_Text objectsHolderCounter;

        private void Update() {
            if(objectsHolderCounter != null) {
                objectsHolderCounter.text = (extraText + " " + objectsHolder.childCount).ToString();
            }
        }

        public void SpawnMyGameObject(GameObject myGameObject) {
            GameObject go = Instantiate(myGameObject, spawnPostion.position, Quaternion.identity, objectsHolder);
        }
    }
}

