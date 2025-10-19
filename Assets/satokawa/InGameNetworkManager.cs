using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(PhotonView))]
/// <summary>
/// インゲームのネットワーク関係を管理
/// </summary>
public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class CloneData
    {
        [Header("クローンするPrefab")] public GameObject clonePrefab;
        [Header("ステージ上にあるクローンする場所")] public Transform clonePosition;
    }
    [SerializeField, Header("プレイヤーPrefab    !!Resourcesフォルダに入れる!!")] private GameObject _playerPrefab;
    [SerializeField, Header("プレイヤーの生成位置")] private Transform[] _playerClonePosition;
    [SerializeField, Header("敵の生成位置と敵オブジェクト")] private CloneData[] _enemyClone;
    [SerializeField, Header("アイテムをの生成位置とアイテムオブジェクト")] private CloneData[] _itemClone;
    public int _playerNumber { get; private set; }//何番目にルームに入ったか
    private bool _isAllLoaded;
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
            _playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, $"isLoaded{_playerNumber}", 1);

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitAllLoaded());
            }
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
    private IEnumerator WaitAllLoaded()
    {
        // 条件が満たされるまで待つ
        yield return new WaitUntil(() => _isAllLoaded);

        // 条件が揃ったらここが実行される
        photonView.RPC(nameof(CreatePlayerTank), RpcTarget.All);
        CreateEnemyTank();
        CreateItem();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            CustomPropertiesManager.SetNetValue(player, $"isLoaded{player.ActorNumber}", 0);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (_isAllLoaded)
        {
            return;
        }
        _isAllLoaded = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int data = (int)CustomPropertiesManager.GetNetValue(player, $"isLoaded{player.ActorNumber}", out bool found);
            if (!found || data == 0)
            {
                _isAllLoaded = false;
                break;
            }
        }

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(Random.Range(-1000, 1000).ToString(), new RoomOptions(), TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        _playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, $"isLoaded{_playerNumber}", 1);
        StartCoroutine(WaitAllLoaded());
    }

    /// <summary>
    /// プレイヤーを生成
    /// </summary>
    [PunRPC]
    public void CreatePlayerTank()
    {
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
    [PunRPC]
    public void ReturnToTitle()
    {
        StartCoroutine(DisconnectAndReturn());
    }

    private IEnumerator DisconnectAndReturn()
    {
        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        SceneManager.LoadScene("Title");
    }
}