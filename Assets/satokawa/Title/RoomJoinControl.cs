using Photon.Realtime;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class RoomJoinControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleNetworkManager _networkManager;
    [SerializeField] protected LocalizationDatas _localizationDatas;

    [Header("RoomCreate")]
    [SerializeField] private Button _createButton;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private LocalizeStringEvent _errorText;

    [Header("RoomJoin")]
    [SerializeField] private Button _joinButton;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private GameObject _roomListPrefab;
    [SerializeField] private TitleUIManager _titleUIManager;
    [SerializeField] private TextMeshProUGUI _noRoom;
    private RoomItemView _selectedRoom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_titleUIManager == null)
        {
            _titleUIManager = FindAnyObjectByType<TitleUIManager>();
        }
        _roomNameInput.onSubmit.AddListener(_ => _createButton.onClick.Invoke());
    }
    public override void OnEnable()
    {
        base.OnEnable();
        _errorText.StringReference = null;
        _roomNameInput.text = "";
        _createButton.interactable = true;
        _joinButton.interactable = false;
    }
    void Update()
    {

    }
    public void RoomCreate()
    {
        if (CheckNameInput(_roomNameInput.text, out LocalizedString errorMessage))
        {//適切なルーム名
            _errorText.StringReference = errorMessage;
            _createButton.interactable = false;
            _networkManager.RoomCreate(_roomNameInput.text, JoinError);
        }
        else
        {
            _errorText.StringReference = errorMessage;
        }
    }
    /// <summary>
    /// InputFieldに入った文字が適切かを確認
    /// </summary>
    /// <param name="roomName">InpuFeildに入ったルーム名</param>
    /// <param name="errorMessage">エラーメッセージ</param>
    /// <returns>true 適切　flase 問題を起こす可能性がある</returns>
    public bool CheckNameInput(string roomName, out LocalizedString errorMessage)
    {
        if (roomName.Length < 1)
        {
            errorMessage = _localizationDatas.TooShortText;
            return false;
        }
        if (roomName.Length > 10)
        {
            errorMessage = _localizationDatas.TooLongText;
            return false;
        }
        if (roomName.Contains(" ") || roomName.Contains("　"))
        {
            errorMessage = _localizationDatas.ContainsSpaces;
            return false;
        }
        if (roomName.Contains("/") || roomName.Contains("\\"))
        {
            errorMessage = _localizationDatas.NotAvailableSymbol;
            return false;
        }
        if (_networkManager.FindRoomName(roomName))
        {
            errorMessage = _localizationDatas.UsedNaming;
            return false;
        }
        errorMessage = null;
        return true;
    }

    public void CreateRoomFailure(LocalizedString message)
    {
        _errorText.StringReference = message;
        _createButton.interactable = true;
    }
    public void ReloadRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < _roomListContent.childCount; i++)
        {
            Destroy(_roomListContent.GetChild(i).gameObject);
        }
        foreach (RoomInfo info in roomList)
        {
            GameObject newPanel = Instantiate(_roomListPrefab, _roomListContent);
            if (newPanel.TryGetComponent(out RoomItemView itemView))
            {
                itemView.SetRoomData(info);
            }

            if (newPanel.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    SelectRoom(itemView);
                    itemView.OutLineActive(true);
                    _titleUIManager.OnButtonClick();
                });
            }
        }
        _noRoom.gameObject.SetActive(roomList.Count == 0);
    }
    public void SelectRoom(RoomItemView room)
    {
        if (room == null)
        {
            return;
        }
        if (_selectedRoom != null)
        {
            _selectedRoom.OutLineActive(false);
        }
        else
        {
            _joinButton.interactable = true;
        }
        _selectedRoom = room;
    }
    public void JoinSelectRoom()
    {
        if (_selectedRoom == null)
        {
            return;
        }
        _networkManager.JoinRoom(_selectedRoom._roomInfo.Name,JoinError);
        _selectedRoom.OutLineActive(false);
        _selectedRoom = null;
    }
    public void JoinError()
    {
        _joinButton.interactable = true;
        LoadingUI.Instance.HideLoading();
    }
}