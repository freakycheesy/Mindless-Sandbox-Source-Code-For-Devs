using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject weaponHolder;
    public Transform weaponPivot;
    public float points;
    public int deathCounter = 0;
    GameManager gameManager;
    WeaponTarget target;
    PhotonView view;
    TMP_Text NameText = null;
    TMP_Text PointText = null;
    TMP_Text DeathText = null;
    [SerializeField] GameObject[] deleteInSandbox;
    [SerializeField] GameObject[] deleteInDeathmatch;
    [SerializeField] PageSwitch primaryWeapon;
    [SerializeField] PageSwitch secondaryWeapon;

    private void Start() {
        view = GetComponent<PhotonView>();
        target = GetComponent<WeaponTarget>();
        gameManager = FindAnyObjectByType<GameManager>();
        NameText = GameObject.FindGameObjectWithTag("nameText").GetComponent<TMP_Text>();
        PointText = GameObject.FindGameObjectWithTag("pointText").GetComponent<TMP_Text>();
        DeathText = GameObject.FindGameObjectWithTag("deathText").GetComponent<TMP_Text>();
    }

    private void Update() {
        if(view.IsMine) {         
            if(NameText != null) {
                NameText.text = view.Owner.NickName;
                PointText.text = Mathf.RoundToInt(points).ToString();
                DeathText.text = deathCounter.ToString();
            }
            if (PhotonNetwork.IsMasterClient && (
                    !PhotonNetwork.CurrentRoom.CustomProperties["n"].Equals(PlayerPrefs.GetString("username")) &&
                    !PhotonNetwork.CurrentRoom.CustomProperties["n"].Equals(PlayerPrefs.GetString("username") + "'s Game") &&
                    !PhotonNetwork.CurrentRoom.CustomProperties["m"].Equals(SceneManager.GetActiveScene().name)
                )) {
                ExitGames.Client.Photon.Hashtable RoomCustomProps = new ExitGames.Client.Photon.Hashtable();
                RoomCustomProps.Add("h", PlayerPrefs.GetString("username"));
                RoomCustomProps.Add("n", PlayerPrefs.GetString("username") + "'s Game");
                RoomCustomProps.Add("m", SceneManager.GetActiveScene().name);
                PhotonNetwork.CurrentRoom.SetCustomProperties(RoomCustomProps);
            }
        }
        else {
            
        }
        if (target.health > 0) {
            if(!weaponPivot.parent.gameObject.activeSelf) {
                view.RPC("ActivateWeaponHolder", RpcTarget.AllBufferedViaServer);
            }
        }
        if(gameManager.gameMode == GameManager.GameMode.Sandbox) {
            foreach(GameObject gO in deleteInSandbox) {
                Destroy(gO);
            }
        }
        else if(gameManager.gameMode == GameManager.GameMode.Deathmatch) {
            foreach (GameObject gO in deleteInDeathmatch) {
                Destroy(gO);
            }
        }
    }
    [PunRPC]
    void ActivateWeaponHolder() {
        weaponHolder.SetActive(true);
    }

    [PunRPC]
    void SelectWeapon(int selectedWeapon) {
        int i = 0;
        weaponPivot.parent.gameObject.SetActive(true);
        view.RPC("ActivateWeaponHolder", RpcTarget.AllBufferedViaServer);
        foreach (Transform weapon in weaponPivot) {
            if (i == selectedWeapon) {
                weapon.gameObject.SetActive(true);
            }
            else {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
    [PunRPC]
    public void SwitchPrimaryWeapon(int selectedPrimary) {
        primaryWeapon.SwitchCategory(selectedPrimary);
    }

    [PunRPC]
    public void SwitchSecondaryWeapon(int selectedSecondary) {
        secondaryWeapon.SwitchCategory(selectedSecondary);
    }

    [PunRPC]
    void GetPoints(float amount) {
        points += amount;
        points = Mathf.RoundToInt(points);
    }

    [PunRPC]
    void SetDeathCounter() {
        deathCounter += 1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(Mathf.RoundToInt(points));
        }
        else {
            points = (int)stream.ReceiveNext();
        }
    }
}
