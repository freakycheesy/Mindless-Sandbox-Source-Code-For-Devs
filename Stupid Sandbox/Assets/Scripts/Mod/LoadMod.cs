using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MindlessMods {
    public class LoadMod : MonoBehaviour {

        public CurrentModInfo currentModInfo;
        public string ModMakerSceneName = "ModMaker";
        public int selectedModIndex = 1;

        public void ChangeModIndex(int index) {
            selectedModIndex = index;
        }

        public void LoadMyMod() {
            CurrentModInfo modItem = Instantiate(currentModInfo, null);
            modItem.selectedModIndex = selectedModIndex;
            DontDestroyOnLoad(modItem);
            Debug.Log("Loaded: Mod " + selectedModIndex);
            StartCoroutine(LoadYourAsyncScene());
        }     
        
        IEnumerator LoadYourAsyncScene() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(ModMakerSceneName);

            while (!asyncLoad.isDone) {
                yield return null;
            }
        }

    }
}

