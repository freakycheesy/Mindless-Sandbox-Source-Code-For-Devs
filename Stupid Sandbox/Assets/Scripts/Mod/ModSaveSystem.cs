using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MindlessMods {
    public class ModSaveSystem : MonoBehaviour {

        public ModGameObject[] objectsSerialized;

        public static List<ModGameObject> objects = new List<ModGameObject>();

        public Transform objectParent;

        const string OBJECT_SUB = "/object";
        const string OBJECT_COUNT_SUB = "/object.count";

        GetModInfo getModInfo;

        void Start() {
            getModInfo = FindAnyObjectByType<GetModInfo>();
            LoadObject();
        }

        void Update() {
            objectsSerialized = objects.ToArray();
        }

        public void SaveObject() {
            BinaryFormatter formatter = new BinaryFormatter();
            Debug.Log(formatter);
            string path = Application.persistentDataPath + OBJECT_SUB + getModInfo.currentModInfo.selectedModIndex;
            Debug.Log(path);
            string countPath = Application.persistentDataPath + OBJECT_COUNT_SUB + getModInfo.currentModInfo.selectedModIndex;
            Debug.Log(countPath);

            FileStream countStream = new FileStream(countPath, FileMode.Create);

            Debug.Log(countStream);

            formatter.Serialize(countStream, objects.Count);
            countStream.Close();

            for(int i = 0; i < objects.Count; i++) {
                FileStream stream = new FileStream(path + i, FileMode.Create);
                Debug.Log(stream);
                ModObjectData data = new ModObjectData(objects[i]);
                Debug.Log(data);
                formatter.Serialize(stream, data);
                stream.Close();
            }
        }

        public void LoadObject() {
            BinaryFormatter formatter = new BinaryFormatter();
            Debug.Log(formatter);
            string path = Application.persistentDataPath + OBJECT_SUB + getModInfo.currentModInfo.selectedModIndex;
            Debug.Log(path);
            string countPath = Application.persistentDataPath + OBJECT_COUNT_SUB + getModInfo.currentModInfo.selectedModIndex;
            Debug.Log(countPath);
            int objectCount = 0;

            if (File.Exists(countPath)) {
                FileStream countStream = new FileStream (countPath, FileMode.Open);
                Debug.Log(countStream);

                objectCount = (int)formatter.Deserialize(countStream);
                countStream.Close();
            }
            else {
                Debug.LogError("Path not found in " + countPath);
            }

            for (int i = 0; i < objects.Count; i++) {
                if (File.Exists(path + i)) {
                    FileStream stream = new FileStream(path + i, FileMode.Open);
                    Debug.Log(stream);
                    ModObjectData data = formatter.Deserialize(stream) as ModObjectData;
                    Debug.Log(data);

                    stream.Close();

                    Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);

                    Quaternion rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

                    Vector3 scale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);

                    ModGameObject modGameObject = Instantiate(Resources.Load(data.m_name.Replace("(Clone)", null)) as ModGameObject, position, rotation, objectParent);
                    Debug.Log(modGameObject);
                    modGameObject.transform.localScale = scale;
                }
                else {
                    Debug.LogError("Path not found in " + path + i);
                }
            }
        }
    }
}