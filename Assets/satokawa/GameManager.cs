using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;
    [SerializeField, Header("リスポーン時間")] private float _respawnTime;
    [SerializeField, Header("残機数")] private int _lives;
    [SerializeField, Header("リザルトマネージャー")] private ResultManager _resultManager;
    [SerializeField, Header("CursorManager")] private CursorManager _cursorManager;
    [SerializeField, Header("PauseControl")] private PauseControl _pauseControl;
    [SerializeField] private TextMeshProUGUI _livesText;
    public List<PlayerController> Players { get; private set; }
    public List<EnemyBase> Enemys { get; private set; }

    private PlayerController _minePlayer;
    private BulletShooter _mineBulletShooter;
    private bool _isRespawnTimer = false;
    private float _respawnTimer;
    private bool _isGameTimer = false;
    private float _gameTimer;
    private InGameNetworkManager _networkManager;
    private static int offllneLives = -1;
    private void Awake()
    {
        _networkManager = GetComponent<InGameNetworkManager>();
        Players = new List<PlayerController>();
        Enemys = new List<EnemyBase>();

         if (PhotonNetwork.InRoom)
        {
            SetupAfterJoiningRoom();
        }
        
        if (_cursorManager == null)
        {   
            _cursorManager = FindAnyObjectByType<CursorManager>();
        }
        if(_resultManager == null)
        {
            _resultManager = FindAnyObjectByType<ResultManager>();
        }
        if(_pauseControl == null)
        {
            _pauseControl = FindAnyObjectByType<PauseControl>();
        }
    }
    public override void OnJoinedRoom()
    {
        SetupAfterJoiningRoom();
    }
    /// <summary>
    /// ルームに入った後のセットアップ
    /// </summary>
    private void SetupAfterJoiningRoom()
    {
        //残機数をCustomPropertyに保存
        //if (PhotonNetwork.IsMasterClient)
        //{

        if (PhotonNetwork.OfflineMode)
        {
            if(offllneLives  == -1)
            {
                offllneLives = _lives;
            }
            _livesText.text = offllneLives.ToString();
            return;
        }
        int lives = (int)NetworkCore.GetNetValue("lives", out bool found);
        if (!found)
        {
            NetworkCore.SetNetValue("lives", _lives);
        }
        _livesText.text = lives.ToString();

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void ToggleTimer(bool b)
    {
        _isGameTimer = b;
    }

    public void Update()
    {
        //リスポーンタイマーを動かし時間になったらNetworkmanagerにPlayerを作ってもらう
        if (_isRespawnTimer)
        {
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer > _respawnTime)
            {
                _networkManager.CreatePlayerTank();
                _isRespawnTimer = false;
            }
        }

        //ポーズ
        if (Input.GetKeyDown(KeyCode.Escape) && _isGameTimer)
        {
            if (_pauseControl.IsShow())
            {
                ClausePause();
            }
            else
            {
                _pauseControl.ShowPanel(true);
                _cursorManager.EnableDefaultCursor();

                if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    Time.timeScale = 0f;
                    _minePlayer.enabled = false;
                    _mineBulletShooter.enabled = false;
                }
            }
        }

        if (_isGameTimer)
        {
            _gameTimer += Time.deltaTime;
        }

        if (PhotonNetwork.AutomaticallySyncScene)
        {
            foreach (EnemyBase enemy in Enemys)
            {
                if (enemy != null && enemy.GetComponent<PhotonView>().IsMine)
                {
                    enemy.Move();
                }
            }
        }
    }

    public void ClausePause()
    {
        _pauseControl.ShowPanel(false);
        _cursorManager.DisableDefaultCursor();

        if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Time.timeScale = 1f;
            _minePlayer.enabled = true;
            _mineBulletShooter.enabled = true;
        }
    }

    /// <summary>
    ///[PunRPC] NetWorkMagagerで生成したプレイヤーを保存する
    /// </summary>
    /// <<param name="newPlayerViewID">photonViewのviewIDを入れる</param>>
    [PunRPC]
    public void AddPlayer(int newPlayerViewID)
    {
        PlayerController newPlayer = GetPlayerController(newPlayerViewID,out PhotonView view);
        if (newPlayer == null)
        {
            Debug.LogError("IDError");
            return;
        }
        if (view.IsMine)
        {
            _minePlayer = newPlayer;
            if(newPlayer.TryGetComponent(out BulletShooter shooter))
            {
                _mineBulletShooter = shooter;
            }
        }
        Players.Add(newPlayer);
    }
    /// <summary>
    /// NetWorkMagagerで生成した敵を保存する
    /// </summary>
    /// <param name="newEnemyViewID">photonViewのviewIDを入れる</param>
    [PunRPC]
    public void AddEnemy(int newEnemyViewID)
    {
        PhotonView targetView = PhotonView.Find(newEnemyViewID);
        if (targetView == null)
        {
            return;
        }
        EnemyBase newEnemy = targetView.GetComponent<EnemyBase>();
        if (newEnemy == null)
        {
            return;
        }
        Enemys.Add(newEnemy);
    }
    /// <summary>
    ///[PunRPC]　プレイヤーのHPを確認して必要に応じてゲームオーバーやリスポーン処理を実行
    /// </summary>
    /// <param name="diePlayerID">photonViewのviewIDを入れる</param>
    [PunRPC]
    public void CheckPlayerActive(int diePlayerID)
    {
        PlayerController diePlayer = GetPlayerController(diePlayerID,out _);
        if (diePlayer == null)
        {
            Debug.LogError("diPlayerID not find");
            return;
        }
        if (diePlayer.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(diePlayer.gameObject);
        }
        bool isPlayerActive = false;
        foreach (PlayerController tank in Players)
        {
            if (tank != null && tank.Hp > 0)
            {
                isPlayerActive = true;
            }
        }

        if (!isPlayerActive)
        {
            if (ReduceLives())
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(Retry), RpcTarget.All);
                }
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    _resultManager.GetComponent<PhotonView>().RPC("ShowGameOverResult", RpcTarget.All);
                }
                _cursorManager.EnableDefaultCursor();
                _isGameTimer = false;
                ClausePause();
                _isRespawnTimer = false;
                _minePlayer.enabled = false;
                _mineBulletShooter.enabled = false;
                
            }
        }
        else if (diePlayer.GetComponent<PhotonView>().IsMine)
        {
            _isRespawnTimer = true;
            _respawnTimer = 0;
        }
    }
    /// <summary>
    /// photonView.viewIDをPlayerController変換
    /// </summary>
    /// <param name="diePlayerID">photonViewのviewIDを入れる</param>
    /// <returns>photonView.viewIDをPlayerController変換した値</returns>
    private PlayerController GetPlayerController(int diePlayerID,out PhotonView targetView)
    {
        targetView = PhotonView.Find(diePlayerID);
        if (targetView == null)
        {
            return null;
        }
        PlayerController player = targetView.GetComponent<PlayerController>();
        return player;
    }
    /// <summary>
    ///[PunRPC] 敵のHPを確認してすべて倒していたらゲームオーバーを実行
    /// </summary>
    [PunRPC]
    public void CheckEnemeyActive()
    {
        bool isEnemyActive = false;
        foreach (EnemyBase tank in Enemys)
        {
            if (tank != null && tank.Hp > 0)
            {
                isEnemyActive = true;
            }
        }
        if (!isEnemyActive )//&& PhotonNetwork.IsMasterClient)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    _resultManager.GetComponent<PhotonView>().RPC("ShowResult", RpcTarget.All, _gameTimer);
                }
                ClausePause();
                _isGameTimer = false;
                _cursorManager.EnableDefaultCursor();
                _isRespawnTimer = false;
                _minePlayer.enabled = false;
                _mineBulletShooter.enabled = false;
             
            });
        }
    }
    

    /// <summary>
    ///[PunRPC] リトライ処理
    /// 現在のステージをリロードする
    /// </summary>

    [PunRPC]
    public void Retry()
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}
        CRIAudioManager.BGM.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //string activeScene = SceneManager.GetActiveScene().name;
        //Debug.Log(activeScene);
        //PhotonNetwork.LoadLevel(activeScene);
    }
    public void ReStart()
    {
        if(PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name == "Stage01")
        {
            photonView.RPC(nameof(Retry), RpcTarget.All);
        }
        CRIAudioManager.BGM.Stop();
        PhotonNetwork.LoadLevel("Stage01");
    }
    /// <summary>
    /// [PunRPC] ゲームオーバー処理 　タイトルに戻す
    /// </summary>

    public void GameOver()
    {
        Time.timeScale = 1f;
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _cursorManager.EnableDefaultCursor();
        photonView.RPC("ReturnToTitle", RpcTarget.All);
        //PhotonNetwork.LoadLevel("Title"); //TODO :　どこのシーンに戻るか決める
    }
    /// <summary>
    /// 残機数を減らし0以下かを確認する
    /// </summary>
    /// <returns>ture 1以上 false 0以下</returns>
    private bool ReduceLives()
    {
        if (PhotonNetwork.OfflineMode)
        {
            offllneLives--;
            _livesText.text = offllneLives.ToString();
            return offllneLives > 0;
        }
        int lives = (int)NetworkCore.GetNetValue("lives", out bool found);
        if (!found)
        {
            Debug.LogError("残機数を取得できませんでした");
        }
        if (lives > 1)
        {
            lives--;
            NetworkCore.SetNetValue("lives", lives);
            return true;
        }
        else
        {
            NetworkCore.SetNetValue("lives", _lives);
            return false;
        }

    }
    /// <summary>
    /// 残機を増やす
    /// </summary>
    /// <param name="value">増加量 基本は1</param>
    public void AddLives(int value = 1)
    {
        if (PhotonNetwork.OfflineMode)
        {
            offllneLives += value;
            _livesText.text = offllneLives.ToString();
            return;
        }
        int livs = (int)NetworkCore.GetNetValue("lives", out bool found);
        if (!found)
        {
            Debug.LogError("残機数を取得できませんでした");
        }
        livs += value;
        NetworkCore.SetNetValue("lives", livs);
    }
    /// <summary>
    ///[PunRPC] ゲームクリア処理　
    /// </summary>
    [PunRPC]
    public void MoveNextScene()
    {
        
        CRIAudioManager.BGM.Stop();
        if (_nextScene == "Title")
        {
            _cursorManager.EnableDefaultCursor();
            _networkManager.ReturnToTitle();
            return;
        }
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.LoadLevel(_nextScene);
    }
    public void GameClear()
    {
        photonView.RPC(nameof(MoveNextScene), RpcTarget.All);
    }
    /// <summary>
    /// カスタムプロパティの変更があったら残機数を取得しUIに表示
    /// </summary>
    /// <param name="propertiesThatChanged"></param>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // 更新されたルームのカスタムプロパティのペアを取得
        foreach (var prop in propertiesThatChanged)
        {
            if (prop.Key is string key && key == "lives")
            {
                _livesText.text = prop.Value.ToString();
            }
        }

    }
    public float GetTime()
    {
        return _gameTimer;
    }
}