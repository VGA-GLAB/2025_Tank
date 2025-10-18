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
    [SerializeField] private TextMeshProUGUI _errorText;

    [Header("RoomJoin")]
    [SerializeField] private Button _joinButton;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private GameObject _roomListPrefab;
    private RoomItemView _selectedRoom;
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
            _errorText.text = errorMessage;
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
        if (roomName.Length < 1 )
        {
            errorMessage = "1文字以上にしてください。";
            return false;
        } 
        if (roomName.Length > 10)
        {
            errorMessage = "10文字以下にしてください。";
            return false;
        }
        if (roomName.Contains(" ") || roomName.Contains("　"))
        {
            errorMessage = "スペースを含めることはできません。";
            return false;
        }
        if (roomName.Contains("/") || roomName.Contains("\\"))
        {
            errorMessage = "/ \\ は使えません";
            return false;
        }
        if (_networkManager.FindRoomName(roomName))
        {
            errorMessage = "このルーム名はすでに使用されています。";
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
        Debug.Log("ルームリスト再読み込み");
        for (int i = 0; i < _roomListContent.childCount; i++)
        {
            Destroy(_roomListContent.GetChild(i).gameObject);
        }
        foreach (RoomInfo info in roomList)
        {
            GameObject newPanel = Instantiate(_roomListPrefab, _roomListContent);
            if(newPanel.TryGetComponent(out RoomItemView itemView))
            {
                itemView.SetRoomData(info);
            }

            if(newPanel.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    SelectRoom(itemView);
                    itemView.OutLineActive(true);
                });
            }
        }
    }
    public void SelectRoom(RoomItemView room)
    {
        if(room == null)
        {
            return;
        }
        if(_selectedRoom != null)
        {
            _selectedRoom.OutLineActive(false);
        }
        _selectedRoom = room;
    }
    public void JoinSelectRoom()
    {
        if(_selectedRoom == null)
        {
            return;
        }
        _networkManager.JoinRoom(_selectedRoom._roomInfo.Name);
        _selectedRoom.OutLineActive(false);
        _selectedRoom = null;
    }
}