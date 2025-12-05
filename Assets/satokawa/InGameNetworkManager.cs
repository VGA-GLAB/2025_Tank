using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
[RequireComponent(typeof(PhotonView))]
/// <summary>
/// インゲームのネットワーク関係を管理
/// </summary>
public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class CloneData
    {
        [Header("クローンするPrefab")]
        public GameObject clonePrefab;

        [Header("ステージ上にあるクローンする場所")]
        public Transform clonePosition;
    }
    [SerializeField, Header("プレイヤーPrefab    !!Resourcesフォルダに入れる!!")]
    private GameObject _playerPrefab;
    [field:SerializeField, Header("プレイヤーのマテリアル")]
    public Material[] _playerMaterials { get; private set; }

    [SerializeField, Header("プレイヤーの生成位置")]
    private Transform[] _playerClonePosition;

    [SerializeField, Header("敵の生成位置と敵オブジェクト")]
    private CloneData[] _enemyClone;

    [SerializeField, Header("アイテムをの生成位置とアイテムオブジェクト")]
    private CloneData[] _itemClone;

    [SerializeField, Header("壊せる壁プレハブ")]
    private GameObject _wallPrefab;

    [SerializeField, Header("PlayerのHPGaugeController")]
    private HPGaugeController _playerHPGauge;

    [SerializeField, Header("ボスのHPゲージ　ボス戦以外はNullにする")]
    private HPGaugeController _bossHPGauge;

    [SerializeField, Header("バレットインターバルゲージ")]
    private AttackIntervalGauge _attackIntervalGauge;

    [SerializeField, Header("バフアイテムUI")]
    private BuffUIManager _buffUIManager; 

    [SerializeField, Header("ゲームマネージャー")]
    private GameManager _gameManager;
    [SerializeField, Header("プレイヤーすべての合計HP")]
    private int _allPlayerHP;
    [SerializeField]
    private CountdownController _countdownController;
    public int _playerNumber { get; private set; }//何番目にルームに入ったか
    private bool _isAllLoaded;

    private List<GameObject> _clonedObjects = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }
        if(_countdownController == null)
        {
            _countdownController = FindAnyObjectByType<CountdownController>();
        }
        if(_attackIntervalGauge == null)
        {
            _attackIntervalGauge = FindAnyObjectByType<AttackIntervalGauge>();
        }
        if(_buffUIManager == null)
        {
            _buffUIManager = FindAnyObjectByType<BuffUIManager>();
        }
    }
    void Start()
    {
        //接続の状態によって処理を分岐
        if (PhotonNetwork.InRoom)
        {
            _playerNumber = NetworkCore.GetPlayerNumber(PhotonNetwork.LocalPlayer);
            NetworkCore.SetNetValue($"isLoaded{PhotonNetwork.LocalPlayer.ActorNumber}", 1);
            OnRoomPropertiesUpdate(null);
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
        photonView.RPC(nameof(CreateWall), RpcTarget.All);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            NetworkCore.SetNetValue($"isLoaded{player.ActorNumber}", 0);
        }
        _countdownController.RequestStartCountdown("StartInGame",photonView);
    }
    /// <summary>
    /// カウントダウンから呼ばれる
    /// </summary>
    [PunRPC]
    public void StartInGame()
    {
        foreach(GameObject obj in _clonedObjects)
        {
            if(obj.TryGetComponent(out PlayerController playerController))
            {
                playerController.enabled = true;
            }
            if(obj.TryGetComponent(out BulletShooter shooter))
            {
                shooter.enabled = true;
            }
            if(obj.TryGetComponent(out EnemyBase enemy))
            {
                enemy.enabled = true;
            }
        }
        _gameManager.ToggleTimer(true);
        CRIAudioManager.BGM.Play("BGM", "bgm_ingame");
    }
    //public void Update()
    //{
    //    if (_isAllLoaded)
    //    {
    //        return;
    //    }
    //    _isAllLoaded = true;
    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        int data = (int)NetworkCore.GetNetValue($"isLoaded{player.ActorNumber}", out bool found);

    //        if (!found || data == 0)
    //        {
    //            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
    //            {
    //                NetworkCore.SetNetValue($"isLoaded{PhotonNetwork.LocalPlayer.ActorNumber}", 1);
    //            }
    //            _isAllLoaded = false;
    //            break;
    //        }
    //    }
    //}

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(Random.Range(-1000, 1000).ToString(), new RoomOptions(), TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        _playerNumber = NetworkCore.GetPlayerNumber(PhotonNetwork.LocalPlayer);
        NetworkCore.SetNetValue($"isLoaded{PhotonNetwork.LocalPlayer.ActorNumber}", 1);
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

        if (newPlayer.TryGetComponent(out PlayerController playerController))
        {
            playerController.HPGauge = _playerHPGauge;
            playerController.BuffUI = _buffUIManager;   
            if (_allPlayerHP != 0)
            {
                int playerHP = _allPlayerHP / PhotonNetwork.PlayerList.Length;
                playerController.SetHP(playerHP);
                playerController.Awake();
            }

            if (newPlayer.TryGetComponent(out BulletShooter bulletShooter))
            {
                bulletShooter.IntervalGauge = _attackIntervalGauge;

                if (!_gameManager.IsGameTimer)
                {
                    bulletShooter.enabled = false;
                    playerController.enabled = false;
                }
            }
        }
       

        //マテリアル変更
        for(int i = 0; i < newPlayer.transform.childCount; i++)
        {
            if(newPlayer.transform.GetChild(i).TryGetComponent(out SkinnedMeshRenderer renderer))
            {
                renderer.material = _playerMaterials[_playerNumber -1];
            }
        }
        _playerHPGauge.SetTarget(newPlayer);
        _clonedObjects.Add(newPlayer);
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
            GameObject newEnemy = PhotonNetwork.InstantiateRoomObject(enemyClone.clonePrefab.name, enemyClone.clonePosition.position, enemyClone.clonePosition.rotation);
            if (newEnemy.TryGetComponent(out EnemyBoss boss))
            {
                photonView.RPC(nameof(SetBossHPGauge),RpcTarget.All, boss.GetComponent<PhotonView>().ViewID);
            }
            if(newEnemy.TryGetComponent(out EnemyBase enemy))
            {
                enemy.enabled = false; 
            }
            _clonedObjects.Add(newEnemy);
            photonView.RPC("AddEnemy", RpcTarget.All, newEnemy.GetComponent<PhotonView>().ViewID);
        }
    }
    [PunRPC]
    public void SetBossHPGauge(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView == null) Debug.LogError("ViewError");
        _bossHPGauge.SetTarget(photonView.gameObject);
        photonView.GetComponent<EnemyBoss>().SetHPGage(_bossHPGauge);
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
            GameObject newItem = PhotonNetwork.InstantiateRoomObject(enemyClone.clonePrefab.name, enemyClone.clonePosition.position, enemyClone.clonePosition.rotation);
        }
    }
    /// <summary>
    /// マスターが壊せる壁を生成
    /// </summary>
    [PunRPC]
    public void CreateWall()
    {

        GameObject[] objects = GameObject.FindGameObjectsWithTag("DestructibleWall");
        if (objects.Length == 0)
        {
            return;
        }
        Transform[] walls = objects.Select(obj => obj.transform).ToArray();
        Transform parent = walls[0].parent;
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        //生成はマスターだけが行う
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform wall in walls)
            {
                GameObject newWall = PhotonNetwork.InstantiateRoomObject(_wallPrefab.name, wall.position, wall.rotation);
                newWall.transform.parent = parent;
                newWall.transform.localScale = wall.localScale;
            }
        }

    }

    [PunRPC]
    public void ReturnToTitle()
    {
        CRIAudioManager.BGM.Stop();
        StartCoroutine(DisconnectAndReturn());
    }

    private IEnumerator DisconnectAndReturn()
    {
        LoadingUI.Instance.ShowLoading("切断中...");
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        SceneManager.LoadScene("Title");
        LoadingUI.Instance.HideLoading();
       
    }
    private bool CheckAllLoaded()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var key = $"isLoaded{player.ActorNumber}";
            float value = NetworkCore.GetNetValue(key, out bool found);

            if (!found || value == 0)
            {
                return false;
            }
        }
        return true;
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _isAllLoaded = CheckAllLoaded();
    }

}