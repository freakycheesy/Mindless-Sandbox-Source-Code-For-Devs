using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MindlessMods {
    public class GetModInfo : MonoBehaviour {

        public CurrentModInfo currentModInfo;

        public TMP_Text modIndexText;

        private void Awake() {
            currentModInfo = FindAnyObjectByType<CurrentModInfo>();

            modIndexText.text = currentModInfo.selectedModIndex.ToString();
        }

    }
}

