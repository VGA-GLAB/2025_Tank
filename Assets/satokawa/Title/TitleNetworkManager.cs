using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
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
    private Dictionary<string, RoomInfo> cachedRoomList = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void JoinMaster()
    {
        _logText.text = "Connecting to server...";
        _logUI.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        _logText.text = "Connecting to lobby...";
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
        _logText.text = "Creating a room...";
        _logUI.SetActive(true);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom(string roomName)
    {
        _logText.text = "Joining a room...";
        _logUI.SetActive(true);
        PhotonNetwork.JoinRoom(roomName);
    }
    /// <summary>
    /// ルームに参加したとき
    /// </summary>
    public override void OnJoinedRoom()
    {
        _logUI.SetActive(false);
        _roomName.text = "RoomName:\n"+PhotonNetwork.CurrentRoom.Name;
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
        foreach(RoomInfo info in roomList)
        {
            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
        _roomList.Clear();
        foreach(RoomInfo info in cachedRoomList.Values)
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
        _logText.text = "Disconnecting...";
        _logUI.SetActive(true);
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        _uIManager.ChangeScreen(0);
        _logUI.SetActive(false);
    }
}