using UnityEngine;

namespace MindlessMods {

    [System.Serializable]
    public class ModObjectData {

        public string m_name;

        public float[] position;
        public float[] rotation;
        public float[] scale;

        public ModObjectData(ModGameObject modGameObject) {
            m_name = modGameObject.name.Replace("(Clone)", "");

            Vector3 pos = modGameObject.transform.localPosition;
            Quaternion rot = modGameObject.transform.localRotation;
            Vector3 scale = modGameObject.transform.localScale;

            // Position
            this.position = new float[]
            {
                pos.x, pos.y, pos.z,
            };


            // Rotation
            this.rotation = new float[]
            {
                rot.x, rot.y, rot.z,
            };

            // Scale
            this.scale = new float[]
            {
                scale.x, scale.y, scale.z,
            };

        }
    }
}
