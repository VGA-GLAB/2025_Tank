using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// タイトルのネットワークを管理
/// </summary>
public class TitleNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleUIManager _uIManager;
    [SerializeField] private GameObject _logUI;
    [SerializeField] private TextMeshProUGUI _logText;
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private RoomJoinControl _roomJoinControl;
    [SerializeField] private TankUIControl _tankUIControl;
    private List<RoomInfo> _roomList = new();
    private Dictionary<string, RoomInfo> _cachedRoomList = new();
    private float _refreshTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            _refreshTimer += Time.deltaTime;
            if (_refreshTimer >= 10f) // 5秒おき
            {
                _refreshTimer = 0;
                PhotonNetwork.JoinLobby(); // 再参加＝RoomList再取得
            }
        }
    }
    public void StartSinglePlay()
    {
        if (PhotonNetwork.IsConnected)//接続済みだったら切断
            PhotonNetwork.Disconnect();

        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene("Stage1");
    }
    public void JoinMaster()
    {
        PhotonNetwork.OfflineMode = false;
        _logText.text = "サーバーに接続中...";
        _logUI.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        _logText.text = "ロビーに接続中...";
        if (PhotonNetwork.OfflineMode)
        {
            return;
        }
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        _logUI.SetActive(false);
        _uIManager.ChangeScreen(1);
    }
    public void RoomCreate(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        _logText.text = "ルームを作成中...";
        _logUI.SetActive(true);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom(string roomName)
    {
        _logText.text = "ルームに接続中...";
        _logUI.SetActive(true);
        PhotonNetwork.JoinRoom(roomName);
    }
    /// <summary>
    /// ルームに参加したとき
    /// </summary>
    public override void OnJoinedRoom()
    {
        _logUI.SetActive(false);
        _roomName.text = "ルーム名:\n"+PhotonNetwork.CurrentRoom.Name;
        _uIManager.ChangeScreen(3);
        _tankUIControl.JoinNewPlayer();
    }
    /// <summary>
    /// ルームの作成に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _logUI.SetActive(false);
        _roomJoinControl.CreateRoomFailure($"ErrorCode:{returnCode.ToString()}  {message}");//TODO これも変える
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _logText.text = $"ErrorCode:{returnCode.ToString()}  {message}";
        _logUI.SetActive(true);
        DOVirtual.DelayedCall(3f,() => _logUI.SetActive(false));//TODO エラーメッセージを出すUIを作ったらそっちに変える
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.InRoom)
        {
            _tankUIControl.JoinNewPlayer();
        }
    }
    public void GameStart()
    {
        //参加不可にしてInGameSceneに移動
        PhotonNetwork.CurrentRoom.IsOpen = false;
        photonView.RPC(nameof(LoadInGameScene), RpcTarget.All);
    }
    [PunRPC]
    public void LoadInGameScene()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Stage1");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"OnRoomListUpdate 呼ばれた: {roomList.Count} 件");
        foreach (var info in roomList)
        {
            Debug.Log($"ルーム: {info.Name}, 削除: {info.RemovedFromList}, 開放: {info.IsOpen}, 表示: {info.IsVisible}");
        }
        foreach (RoomInfo info in roomList)
        {
            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                {
                    _cachedRoomList.Remove(info.Name);
                }
                continue;
            }
            if (_cachedRoomList.ContainsKey(info.Name))
            {
                _cachedRoomList[info.Name] = info;
            }
            else
            {
                _cachedRoomList.Add(info.Name, info);
            }
        }
        _roomList.Clear();
        foreach(RoomInfo info in _cachedRoomList.Values)
        {
            _roomList.Add(info);
        }
        _roomJoinControl.ReloadRoomList(_roomList);
    }
    public bool FindRoomName(string roomName)
    {
       return  _roomList.Any(_roomList => _roomList.Name == roomName);
    }
    public void ExitRoom()
    {
        _logText.text = "切断中...";
        _logUI.SetActive(true);
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        _uIManager.ChangeScreen(0);
        _logUI.SetActive(false);
    }
}