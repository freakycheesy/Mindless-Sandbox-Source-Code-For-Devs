using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MindlessMods {
    public class ModEditObjectProperties : MonoBehaviour {

        public GameObject selectedGameObject;
        public Transform selectedTransform;

        // TMPs
        public TMP_Text objectName_TMP;
        
        // Position
        public TMP_InputField posX_TMP;
        public TMP_InputField posY_TMP;
        public TMP_InputField posZ_TMP;

        // Rotation
        public TMP_InputField rotX_TMP;
        public TMP_InputField rotY_TMP;
        public TMP_InputField rotZ_TMP;

        // Scale
        public TMP_InputField scaleX_TMP;
        public TMP_InputField scaleY_TMP;
        public TMP_InputField scaleZ_TMP;

        void Update() {
            RaycastHit hit;
            bool hasHitGameObject = Physics.Raycast(transform.position, transform.forward, out hit, 5f);

            if (selectedGameObject != null && Keyboard.current.eKey.wasPressedThisFrame) {
                hasHitGameObject = false;
                selectedGameObject = null;
                selectedTransform = null;

                objectName_TMP.text = "Object Name";

                posX_TMP.text = "";
                posY_TMP.text = "";
                posZ_TMP.text = "";

                rotX_TMP.text = "";
                rotY_TMP.text = "";
                rotZ_TMP.text = "";

                scaleX_TMP.text = "";
                scaleY_TMP.text = "";
                scaleZ_TMP.text = "";
            }

            if ((hasHitGameObject && hit.collider.GetComponent<Grabbable>()) && Keyboard.current.eKey.wasPressedThisFrame) {
                selectedGameObject = hit.collider.gameObject;
                selectedTransform = selectedGameObject.transform;

                objectName_TMP.text = selectedGameObject.name;

                posX_TMP.text = selectedTransform.localPosition.x.ToString();
                posY_TMP.text = selectedTransform.localPosition.y.ToString();
                posZ_TMP.text = selectedTransform.localPosition.z.ToString();

                rotX_TMP.text = selectedTransform.localRotation.x.ToString();
                rotY_TMP.text = selectedTransform.localRotation.y.ToString();
                rotZ_TMP.text = selectedTransform.localRotation.z.ToString();

                scaleX_TMP.text = selectedTransform.localScale.x.ToString();
                scaleY_TMP.text = selectedTransform.localScale.y.ToString();
                scaleZ_TMP.text = selectedTransform.localScale.z.ToString();
            }          

            if(selectedGameObject != null) {               
                if (posX_TMP.text != "" && posY_TMP.text != "" && posZ_TMP.text != "")
                    selectedTransform.localPosition = new Vector3(float.Parse(posX_TMP.text), float.Parse(posY_TMP.text), float.Parse(posZ_TMP.text));
                if (rotX_TMP.text != "" && rotY_TMP.text != "" && rotZ_TMP.text != "")
                    selectedTransform.localRotation = Quaternion.Euler(new Vector3(float.Parse(rotX_TMP.text), float.Parse(rotY_TMP.text), float.Parse(rotZ_TMP.text)));
                if (scaleX_TMP.text != "" && scaleY_TMP.text != "" && scaleZ_TMP.text != "")
                    selectedTransform.localScale = new Vector3(float.Parse(scaleX_TMP.text), float.Parse(scaleY_TMP.text), float.Parse(scaleZ_TMP.text));
            }
        }
    }
}