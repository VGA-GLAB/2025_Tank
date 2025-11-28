using System.Security.Cryptography;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomItemView : MonoBehaviour
{
    [SerializeField] private GameObject _outLine;
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private TextMeshProUGUI _playerCount;
    [SerializeField] private SelectAnimation _selectAnimation;
    public RoomInfo _roomInfo { get; private set; }
    public void Start()
    {
        OutLineActive(false);
        _selectAnimation.SetLoopAnimation(false);
    }
    public void SetRoomData(RoomInfo info)
    {
        _roomName.text = info.Name;
        _playerCount.text = info.PlayerCount + "/" + info.MaxPlayers;
        _roomInfo = info;
    }
    public void OutLineActive(bool a)
    {
        _outLine.SetActive(a);
        _selectAnimation.SetLoopAnimation(a);
    }
}
