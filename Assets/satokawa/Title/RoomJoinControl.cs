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
        _errorText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RoomCreate()
    {
        if (CheckNameInput(_roomNameInput.text, out string errorMessage))
        {//適切なルーム名
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
            errorMessage = "ルーム名は1文字以上10文字以下にしてください";
            return false;
        }
        if (roomName.Contains(" ") || roomName.Contains("　"))
        {
            errorMessage = "ルーム名にスペースは使用できません";
            return false;
        }
        if (roomName.Contains("/") || roomName.Contains("\\"))
        {
            errorMessage = "ルーム名に / \\ は使用できません";
            return false;
        }
        if (_networkManager.FindRoomName(roomName))
        {
            errorMessage = "そのルーム名はすでに使用されています";
            return false;
        }
        errorMessage = "";
        return true;

    }
}