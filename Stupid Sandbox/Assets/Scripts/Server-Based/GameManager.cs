using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public float maxPoints;
    public TMP_Text maxPoint_TMP;

    public enum GameMode {
        Sandbox = 0,
        Deathmatch = 1,
    }

    public GameMode gameMode;

    public GameObject pointsUIElement;

    public SpawnMenu spawnMenu;

    public GameObject[] onlySandboxElements;

    public GameObject[] onlyDeathmatchElements;

    public GameObject[] hostPreJoinAndClientPreJoin;

    public UnityEvent gameEndEvent;

    public Animator gameEndAnimator;

    public TMP_Text gameEndText;

    public PlayerList playerList;

    public PreviewCamera previewCamera;

    public Button spawnButton;

    public bool isStarted;

    public bool gameRunning;

    private AudioSource musicSource;

    public float maxTimer = 120f;

    public float timer = 120f;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        playerList = FindAnyObjectByType<PlayerList>();
        previewCamera = FindAnyObjectByType<PreviewCamera>();

        if (maxPoint_TMP != null) {
            maxPoint_TMP.text = maxPoints.ToString("0,000");
        }
        if (gameMode == GameMode.Sandbox) {
            previewCamera.enabled = false;
            musicSource.Stop();
            musicSource.enabled = false;
            isStarted = true;
            gameRunning = true;
            spawnMenu.enabled = true;
            hostPreJoinAndClientPreJoin[0].SetActive(false);
            hostPreJoinAndClientPreJoin[1].SetActive(true);
            pointsUIElement.SetActive(false);
            spawnButton.interactable = true;
            for (int i = 0; i < onlySandboxElements.Length; i++) {
                onlySandboxElements[i].SetActive(true);
            }             
        }
        else if (gameMode != GameMode.Sandbox) {
            previewCamera.enabled = true;
            isStarted = false;
            musicSource.Play();
            musicSource.enabled = true;
            for (int i = 0; i < onlySandboxElements.Length; i++) {
                onlySandboxElements[i].SetActive(false);
            }
        }
        if(gameMode == GameMode.Deathmatch) {
            pointsUIElement.SetActive(true);
            spawnMenu.enabled = true;
            hostPreJoinAndClientPreJoin[0].SetActive(PhotonNetwork.IsMasterClient);
            hostPreJoinAndClientPreJoin[1].SetActive(!PhotonNetwork.IsMasterClient);
            if (PhotonNetwork.IsMasterClient) {
                PhotonView view = GetComponent<PhotonView>();
                view.RequestOwnership();
            }
        }
        else if (gameMode != GameMode.Deathmatch) {
            for (int i = 0; i < onlyDeathmatchElements.Length; i++) {
                onlyDeathmatchElements[i].SetActive(false);
            }
        }
    }

    public void StartGame() {
        PhotonView view = GetComponent<PhotonView>();
        isStarted = true;
        spawnButton.interactable = true;
        view.RPC("StartGameRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void StartGameRPC() {
        isStarted = true;
        gameRunning = true;
        spawnButton.interactable = true;
        timer = maxTimer;
    }

    [PunRPC]
    void EndGameRPC() {
        isStarted = false;
        gameRunning = false;
        spawnButton.interactable = false;
        timer = maxTimer;
    }


    void EndGame() {
        PhotonNetwork.DestroyAll();
        playerList.highPoints = 0;
        gameEndEvent.Invoke();
        Cursor.lockState = CursorLockMode.None;
    }

    public void ChangeMaxPoints(float value) {
        if (maxPoint_TMP != null) {
            PhotonView view = GetComponent<PhotonView>();
            view.RPC("ChangeMaxPointsRPC", RpcTarget.AllBufferedViaServer, value);
        }
    }

    [PunRPC]
    public void ChangeMaxPointsRPC(float value) {
        maxPoints = Mathf.RoundToInt(value);
        maxPoint_TMP.text = maxPoints.ToString("0,000");
    }

    public void ChangeMaxTimer(float value) {
        if (maxPoint_TMP != null) {
            PhotonView view = GetComponent<PhotonView>();
            view.RPC(nameof(ChangeMaxTimerRPC), RpcTarget.AllBufferedViaServer, value);
        }
    }

    [PunRPC]
    public void ChangeMaxTimerRPC(float value) {
        maxTimer = Mathf.RoundToInt(value);
        timer = maxTimer;
        maxPoint_TMP.text = maxPoints.ToString("0,000") + "/" + Mathf.RoundToInt(timer);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            if(PhotonNetwork.IsMasterClient) {
                stream.SendNext(maxPoints);
                stream.SendNext(isStarted);
                stream.SendNext(gameRunning);
                stream.SendNext(spawnButton.interactable);
                stream.SendNext(timer);
            }
        }
        else {
            maxPoints = (float)stream.ReceiveNext();
            isStarted = (bool)stream.ReceiveNext();
            gameRunning = (bool)stream.ReceiveNext();
            spawnButton.interactable = (bool)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();
        }
    }
    private void Update() {
        if (gameMode == GameMode.Sandbox) {
            // Host Migration
            if (hostPreJoinAndClientPreJoin[0] != null) {
                hostPreJoinAndClientPreJoin[0].SetActive(false);
            }
            if (hostPreJoinAndClientPreJoin[1]) {
                hostPreJoinAndClientPreJoin[1].SetActive(true);
            }
            if (PhotonNetwork.IsMasterClient) {
                PhotonView view = GetComponent<PhotonView>();
                view.RequestOwnership();
            }
        }
        if(gameMode == GameMode.Deathmatch) {
            if (gameRunning) {
                if (PhotonNetwork.IsMasterClient) {
                    timer -= Time.deltaTime;
                }
                maxPoint_TMP.text = maxPoints.ToString("0,000") + "/" + Mathf.RoundToInt(timer);
                if (timer <= 0) {
                    timer = 0;
                }
            }
            if ((playerList.highPoints >= maxPoints && gameRunning) || (timer <= 0)) {
                gameEndAnimator.Play("Splash");
                gameEndText.text = playerList.Text_playerName.text + " " + "Wins";
                PhotonView view = GetComponent<PhotonView>();
                view.RPC("EndGameRPC", RpcTarget.AllBufferedViaServer);
                Invoke(nameof(EndGame), 3.5f);
            }
            if (playerList.highPoints >= maxPoints / 1.25f) {
                AudioSource music = GetComponent<AudioSource>();
                music.pitch = 1.25f;
            }
            else {
                AudioSource music = GetComponent<AudioSource>();
                music.pitch = 1f;
            }
            // Host Migration
            hostPreJoinAndClientPreJoin[0].SetActive(PhotonNetwork.IsMasterClient);
            hostPreJoinAndClientPreJoin[1].SetActive(!PhotonNetwork.IsMasterClient);
            if (PhotonNetwork.IsMasterClient) {
                PhotonView view = GetComponent<PhotonView>();
                view.RequestOwnership();
            }
        }
    }
}