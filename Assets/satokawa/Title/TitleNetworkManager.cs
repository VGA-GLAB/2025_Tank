using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private ErrorMessageUI _messageUI;
    [SerializeField] private GameObject _logUI;
    [SerializeField] private TextMeshProUGUI _logText;
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private RoomJoinControl _roomJoinControl;
    [SerializeField] private TankUIControl _tankUIControl;
    [SerializeField] private Button _serverJoinButton;
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
      
    }
    public void RoomDataReload()
    {
        PhotonNetwork.JoinLobby();
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
        _serverJoinButton.interactable = false;
        //インターネット接続確認
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _messageUI.ShowMessage("インターネットに接続されていません。");
            return;
        }

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
        _cachedRoomList.Clear();   
        _roomList.Clear();         

        _serverJoinButton.interactable = true;
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

        Debug.Log(PhotonNetwork.IsMasterClient ? "マスター" : "非マスター" + PhotonNetwork.LocalPlayer.ActorNumber);
        _logUI.SetActive(false);
        _roomName.text = "ルーム名:\n" + PhotonNetwork.CurrentRoom.Name;
        _uIManager.ChangeScreen(3);
        _tankUIControl.JoinNewPlayer();
        PhotonNetwork.AutomaticallySyncScene = true; // 事前に設定してもOK
    }
    /// <summary>
    /// ルームの作成に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
  
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

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("Stage01");           // マスターだけ呼ぶ
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info.Name);
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
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
        foreach (RoomInfo info in _cachedRoomList.Values)
        {
            _roomList.Add(info);
        }
        _roomJoinControl.ReloadRoomList(_roomList);
    }
    public bool FindRoomName(string roomName)
    {
        return _roomList.Any(_roomList => _roomList.Name == roomName);
    }
    public void ExitRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            OnDisconnected(DisconnectCause.None);
            return;
        }
        _logText.text = "切断中...";
        _logUI.SetActive(true);
        PhotonNetwork.Disconnect();
    }
    //-------------------------------
    //エラー処理
    //-------------------------------

    /// <summary>
    /// サーバーから切断されたとき
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        _serverJoinButton.interactable = true;
        _messageUI.ShowMessage(cause);
        _uIManager.ChangeScreen(0);
        _logUI.SetActive(false);
    }
    /// <summary>
    /// ルームの作成に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _messageUI.ShowMessage($"Error Code:{returnCode.ToString()} \n {message}");
    }
    /// <summary>
    /// ルームの参加に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _messageUI.ShowMessage($"Error Code:{returnCode.ToString()} \n {message}");
    }
    
}