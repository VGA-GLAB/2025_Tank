using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;
    [SerializeField, Header("リスポーン時間")] private float _respawnTime;
    [SerializeField, Header("残機数")] private int _lives;
    public List<PlayerController> Players { get; private set; }
    public List<EnemyBase> Enemys { get; private set; }

    private bool _isRespawnTimer = false;
    private float _timer;
    private InGameNetworkManager _networkManager;
    private void Awake()
    {
        _networkManager = GetComponent<InGameNetworkManager>();
        Players = new List<PlayerController>();
        Enemys = new List<EnemyBase>();

        if (PhotonNetwork.InRoom)
        {
            SetupAfterJoiningRoom();
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
        if (PhotonNetwork.IsMasterClient)
        {
            CustomPropertiesManager.GetNetValue(PhotonNetwork.LocalPlayer, "lives", out bool found);
            if (!found)
            {
                CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, "lives", _lives);
            }
        }
    }
    public void Update()
    {
        //リスポーンタイマーを動かし時間になったらNetworkmanagerにPlayerを作ってもらう
        if (_isRespawnTimer)
        {
            _timer += Time.deltaTime;
            if (_timer > _respawnTime)
            {
                _networkManager.CreatePlayerTank();
                _isRespawnTimer = false;
            }
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
    /// <summary>
    ///[PunRPC] NetWorkMagagerで生成したプレイヤーを保存する
    /// </summary>
    /// <<param name="newPlayerViewID">photonViewのviewIDを入れる</param>>
    [PunRPC]
    public void AddPlayer(int newPlayerViewID)
    {
        PlayerController newPlayer = GetPlayerController(newPlayerViewID);
        if (newPlayer == null)
        {
            Debug.LogError("IDError");
            return;
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
        PlayerController diePlayer = GetPlayerController(diePlayerID);
        if (diePlayer == null)
        {
            return;
        }

        bool isPlayerActive = false;
        foreach (PlayerController tank in Players)
        {
            if (tank != null && tank.Hp > 0)
            {
                isPlayerActive = true;
            }
        }
        if (!isPlayerActive && PhotonNetwork.IsMasterClient)
        {
            if (ReduceLives())
            {
                Retry();
            }
            else
            {
                GameOver();
            }
        }
        else if (diePlayer.GetComponent<PhotonView>().IsMine)
        {
            _isRespawnTimer = true;
            _timer = 0;
        }
    }
    /// <summary>
    /// photonView.viewIDをPlayerController変換
    /// </summary>
    /// <param name="diePlayerID">photonViewのviewIDを入れる</param>
    /// <returns>photonView.viewIDをPlayerController変換した値</returns>
    private PlayerController GetPlayerController(int diePlayerID)
    {
        PhotonView targetView = PhotonView.Find(diePlayerID);
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
        //マスターのみ実行
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        bool isEnemyActive = false;
        foreach (EnemyBase tank in Enemys)
        {
            if (tank != null && tank.Hp > 0)
            {
                isEnemyActive = true;
            }
        }
        if (!isEnemyActive && PhotonNetwork.IsMasterClient)
        {
            GameClear();
        }
    }


    /// <summary>
    ///[PunRPC] リトライ処理
    /// 現在のステージをリロードする
    /// </summary>
    private void Retry()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// [PunRPC] ゲームオーバー処理 　タイトルに戻す
    /// </summary>
    private void GameOver()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.LoadLevel("Title"); //TODO :　どこのシーンに戻るか決める
    }
    /// <summary>
    /// 残機数を減らし0以下かを確認する
    /// </summary>
    /// <returns>ture 1以上 false 0以下</returns>
    private bool ReduceLives()
    {
        int lives = (int)CustomPropertiesManager.GetNetValue(PhotonNetwork.LocalPlayer, "lives", out bool found);
        if (!found)
        {
            Debug.LogError("残機数を取得できませんでした");
        }
        if (lives > 0)
        {
            lives--;
            CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, "lives", lives);
            return true;
        }
        else
        {
            CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, "lives", _lives);
            return false;
        }
    }
    /// <summary>
    /// 残機を増やす
    /// </summary>
    /// <param name="value">増加量 基本は1</param>
    public void AddLives(int value = 1)
    {
        int livs = (int)CustomPropertiesManager.GetNetValue(PhotonNetwork.LocalPlayer, "lives", out bool found);
        if (!found)
        {
            Debug.LogError("残機数を取得できませんでした");
        }
        livs += value;
        CustomPropertiesManager.SetNetValue(PhotonNetwork.LocalPlayer, "lives", livs);
    }
    /// <summary>
    ///[PunRPC] ゲームクリア処理　
    /// </summary>
    private void GameClear()
    {
        if(_nextScene == "Title")
        {
            photonView.RPC("ReturnToTitle", RpcTarget.All);
            return;
        }
        PhotonNetwork.LoadLevel(_nextScene);
    }
}