using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
/// <summary>
/// タイトルのネットワークを管理
/// </summary>
public class TitleNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleUIManager _uIManager;
    [SerializeField] private ErrorMessageUI _messageUI;
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private RoomJoinControl _roomJoinControl;
    [SerializeField] private TankUIControl _tankUIControl;
    [SerializeField] private Button _serverJoinButton;
    [SerializeField] private LocalizationDatas _localizationDatas;
    [SerializeField] private string _fastStage;
    private List<RoomInfo> _roomList = new();
    private Dictionary<string, RoomInfo> _cachedRoomList = new();
    private float _refreshTimer = 0;
    private UnityAction _roomJoinErrorEvent;
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
        StartCoroutine(StartSinglePlayCoroutine());
    }
    public IEnumerator StartSinglePlayCoroutine()
    {
        LoadingUI.Instance.ShowLoading(_localizationDatas.StartGame);

        yield return null;

        if (PhotonNetwork.IsConnected)
        {
            //接続済みだったら切断
            Debug.Log("切断");
           PhotonNetwork.Disconnect();
           yield return new WaitUntil(() => !PhotonNetwork.IsConnected); 
        }

        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene(_fastStage);
    }
    public void JoinMaster()
    {
        _serverJoinButton.interactable = false;
        //インターネット接続確認
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _messageUI.ShowMessage(_localizationDatas.NoInternetConnection);
            _serverJoinButton.interactable = true;
            return;
        }

        PhotonNetwork.OfflineMode = false;

        LoadingUI.Instance.ShowLoading(_localizationDatas.ConnectingToServer);

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {

        if (PhotonNetwork.OfflineMode)
        {
            return;
        }

        LoadingUI.Instance.ShowLoading(_localizationDatas.ConnectingToLobby);
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        _cachedRoomList.Clear();   
        _roomList.Clear();         

        _serverJoinButton.interactable = true;
        LoadingUI.Instance.HideLoading();
        _uIManager.ChangeScreen(1);
        Debug.Log("サーバーに接続済み：" + PhotonNetwork.LocalPlayer.ActorNumber);
    }
    public void RoomCreate(string roomName, UnityAction errorEvent)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        _roomJoinErrorEvent = errorEvent;

        LoadingUI.Instance.ShowLoading(_localizationDatas.CreatingRoom);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom(string roomName,UnityAction errorEvent)
    {
        _roomJoinErrorEvent = errorEvent;

        LoadingUI.Instance.ShowLoading(_localizationDatas.RoomConnecting);
        PhotonNetwork.JoinRoom(roomName);
    }
    /// <summary>
    /// ルームに参加したとき
    /// </summary>
    public override void OnJoinedRoom()
    {
        LoadingUI.Instance.HideLoading();
        _roomName.text = PhotonNetwork.CurrentRoom.Name;
        _uIManager.ChangeScreen(3);
        _tankUIControl.UpdateViewPlayer();
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
            _tankUIControl.UpdateViewPlayer();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _tankUIControl.UpdateViewPlayer();
    }
    public void GameStart()
    {
        //参加不可にしてInGameSceneに移動
        LoadingUI.Instance.ShowLoading(_localizationDatas.StartGame);
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(_fastStage);           // マスターだけ呼ぶ
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
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
        LoadingUI.Instance.ShowLoading(_localizationDatas.Disconnect);
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
        LoadingUI.Instance.HideLoading();
    }
    /// <summary>
    /// ルームの作成に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _messageUI.ShowMessage(_localizationDatas.OperationNotAllowedInCurrentState);
        _roomJoinErrorEvent.Invoke();
    }
    /// <summary>
    /// ルームの参加に失敗したとき
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _messageUI.ShowMessage(_localizationDatas.OperationNotAllowedInCurrentState);
        _roomJoinErrorEvent.Invoke();
    }
    
}