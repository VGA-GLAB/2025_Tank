using TMPro;
using UnityEngine;

public class RoomItemView : MonoBehaviour
{
    [SerializeField] public GameObject _outLine { get;}
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private TextMeshProUGUI _playerCount;

    public void SetRoomData(string name,int playerCount, int maxPlayer)
    {
        _roomName.text = name;
        _playerCount.text = playerCount + "/" + maxPlayer;
    }
}
