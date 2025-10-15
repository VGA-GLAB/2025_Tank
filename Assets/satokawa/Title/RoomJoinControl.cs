using Photon.Realtime;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomJoinControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleNetworkManager _networkManager;

    [Header("RoomCreate")]
    [SerializeField] private Button _createButton;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private GameObject _roomCreate;
    [SerializeField] private TextMeshProUGUI _errorText;

    [Header("RoomJoin")]
    [SerializeField] private Button _joinButton;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private GameObject _roomListPrefab;
    [SerializeField] private GameObject _roomIn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public override void OnEnable()
    {
        base.OnEnable();
        _errorText.text = "";
        _roomNameInput.text = "";
        _createButton.interactable = true;
    }
    void Update()
    {

    }
    public void RoomCreate()
    {
        if (CheckNameInput(_roomNameInput.text, out string errorMessage))
        {//適切なルーム名
            _createButton.interactable = false;
            _networkManager.RoomCreate(_roomNameInput.text);
        }
        else
        {
            _errorText.text = errorMessage;
        }
    }
    /// <summary>
    /// InputFieldに入った文字が適切かを確認
    /// </summary>
    /// <param name="roomName">InpuFeildに入ったルーム名</param>
    /// <param name="errorMessage">エラーメッセージ</param>
    /// <returns>true 適切　flase 問題を起こす可能性がある</returns>
    public bool CheckNameInput(string roomName, out string errorMessage)
    {
        if (roomName.Length < 1 || roomName.Length > 10)
        {
            errorMessage = "Room name must be between 1 and 10 characters.";
            return false;
        }
        if (roomName.Contains(" ") || roomName.Contains("　"))
        {
            errorMessage = "Spaces are not allowed in the room name.";
            return false;
        }
        if (roomName.Contains("/") || roomName.Contains("\\"))
        {
            errorMessage = "Room name cannot contain '/' or '\\'.";
            return false;
        }
        if (_networkManager.FindRoomName(roomName))
        {
            errorMessage = "This room name is already in use.";
            return false;
        }
        errorMessage = "";
        return true;


    }
    public void CreateRoomFailure(string message)
    {
        _errorText.text = message;
        _createButton.interactable = true;
    }
    public void ReloadRoomList(List<RoomInfo> roomList)
    {

    }
}