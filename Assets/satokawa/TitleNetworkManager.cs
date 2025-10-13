using System.Collections.Generic;
using System.Linq;
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
    private List<RoomInfo> _roomList = new();

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
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        _logUI.SetActive(false);
        _uIManager.ChangeScreen(3);
    }
    public void GameStart()
    {
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
        _roomList = roomList;
    }
    public bool FindRoomName(string roomName)
    {
       return  _roomList.Any(_roomList => _roomList.Name == roomName);
    }
}