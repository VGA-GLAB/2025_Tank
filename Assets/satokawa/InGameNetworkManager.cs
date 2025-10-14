using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
[RequireComponent(typeof(PhotonView))]
/// <summary>
/// インゲームのネットワーク関係を管理
/// </summary>
public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class CloneData
    {
        [SerializeField,Header("クローンするPrefab")] public GameObject clonePrefab;
        [SerializeField,Header("ステージ上にあるクローンする場所")] public Transform clonePosition;
    }
    [SerializeField, Header("プレイヤーPrefab    !!Resourcesフォルダに入れる!!")] private GameObject _playerPrefab;
    [SerializeField, Header("プレイヤーの生成位置")] private Transform[] _playerClonePosition;
    [SerializeField, Header("敵の生成位置と敵オブジェクト")] private CloneData[] _enemyClone;
    [SerializeField, Header("アイテムをの生成位置とアイテムオブジェクト")] private CloneData[] _itemClone;
    public int _playerNumber { get; private set; }//何番目にルームに入ったか
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        //接続の状態によって処理を分岐
        if (PhotonNetwork.InRoom)
        {
            CreatePlayerTank();
            CreateEnemyTank();
            CreateItem();
        }
        else if (PhotonNetwork.IsConnected)
        {

            OnConnectedToMaster();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        CreatePlayerTank();
        CreateEnemyTank();
        CreateItem();
    }

    /// <summary>
    /// プレイヤーを生成
    /// </summary>
    public void CreatePlayerTank()
    {
        _playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        Vector3 position;
        Quaternion rotation;
        //プレイヤーの数が_clonePositionを越えていないかを確認
        if (_playerNumber <= _playerClonePosition.Length)
        {
            position = _playerClonePosition[_playerNumber - 1].position;
            rotation = _playerClonePosition[_playerNumber - 1].rotation;
        }
        //超えていたらランダムな場所にする
        else
        {
            position = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
            rotation = Quaternion.identity;
        }

        GameObject newPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, position, rotation);
        PhotonView view = newPlayer.GetComponent<PhotonView>();
        DOVirtual.DelayedCall(0.1f, () => photonView.RPC("AddPlayer", RpcTarget.All, view.ViewID));//TODO:今はゴリ押しでやってるけどタイトルできたらちゃんと書く
    }
    /// <summary>
    /// マスターのみが敵を生成
    /// </summary>
    public void CreateEnemyTank()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        foreach (CloneData enemyClone in _enemyClone)
        {
            GameObject newEnemy = PhotonNetwork.Instantiate(enemyClone.clonePrefab.name, enemyClone.clonePosition.position, enemyClone.clonePosition.rotation);
            photonView.RPC("AddEnemy", RpcTarget.All, newEnemy.GetComponent<PhotonView>().ViewID);
        }
    }
    /// <summary>
    /// マスターのみがアイテムを生成
    /// </summary>
    public void CreateItem()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        foreach (CloneData enemyClone in _itemClone)
        {
            GameObject newItem = PhotonNetwork.Instantiate(enemyClone.clonePrefab.name, enemyClone.clonePosition.position, enemyClone.clonePosition.rotation);
        }
    }
}