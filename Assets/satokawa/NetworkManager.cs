using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
/// <summary>
/// ネットワーク関係を管理
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("プレイヤーPrefab    !!Resourcesフォルダに入れる!!")] private GameObject _tankPrefab;
    [SerializeField, Header("プレイヤーの生成位置")] private Transform[] _clonePosition;

    private GameManager _gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GetComponent<GameManager>();

        //Titleで接続した場合はそのまま生成し未接続の場合は接続する
        if (PhotonNetwork.IsConnected)
        {
            CreatePlayerTank();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        CreatePlayerTank();
    }

    /// <summary>
    /// プレイヤーを生成
    /// </summary>
    private void CreatePlayerTank()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Vector3 position;
        Debug.Log(playerCount);
        //プレイヤーの数が_clonePositionを越えていないかを確認
        if (playerCount <= _clonePosition.Length)
        {
            position = _clonePosition[playerCount -1].position;
        }
        //超えていたらランダムな場所にする
        else
        {
            position = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
        }

       GameObject newPlayer =  PhotonNetwork.Instantiate(_tankPrefab.name, position, Quaternion.identity);
        _gameManager.AddPlayer(newPlayer.GetComponent<PlayerController>());
    }
}