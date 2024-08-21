using Photon.Pun;
using TMPro;
using UnityEngine;

public class UsernameScript : MonoBehaviour {
    [SerializeField]
    PhotonView view;
    [SerializeField]
    TMP_Text usernameTMP;

    public SkinnedMeshRenderer playerMeshRenderer;

    public string[] specialNames =
    {
        "cheesy"
    };

    public Transform cameraPos;

    public Material[] specialMaterials;

    public GameObject[] playerModels;

    private bool canSendUsername = true;

    private void Start() {
        view = GetComponent<PhotonView>();
        if (!view.IsMine) return;       
        PhotonNetwork.NickName = PlayerPrefs.GetString("username") + Random.Range(0, 9999);
        view.Owner.NickName = PlayerPrefs.GetString("username");
    }

    private void Update()
    {
        if (canSendUsername) {
            canSendUsername = false;
            view.RPC("Username", RpcTarget.AllBuffered);
            Invoke(nameof(ResetUsernameDelay), 5f);
        }
        if (usernameTMP == null) return;
        if (cameraPos == null) {
            cameraPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        }
        if (cameraPos != null) {
            usernameTMP.gameObject.transform.parent.LookAt(cameraPos.parent);
        }
        if (view.IsMine && usernameTMP.transform.parent.gameObject != null) {        
            Destroy(usernameTMP.transform.parent.gameObject);
            usernameTMP = null;
        }
    }

    private void ResetUsernameDelay() {
        canSendUsername = true;
    }

    [PunRPC]
    void Username() {
        if (usernameTMP != null) {
            usernameTMP.text = view.Owner.NickName;
        }
        for (int i = 0; i < specialMaterials.Length; i++) {
             if (view.Owner.NickName.Contains(specialNames[i])) {
                playerMeshRenderer.material = specialMaterials[i];
             }
        }
       if (view.Owner.NickName == specialNames[specialNames.Length - 1]) {
            playerModels[0].SetActive(true);
            playerModels[1].SetActive(false);
       }
       else {
            playerModels[0].SetActive(false);
            playerModels[1].SetActive(true);
       }    
    }
}
