using Photon.Pun;
using UnityEngine;
/// <summary>
/// Bullet本体につける
/// Bulletを動かしHIt判定を送る
/// 生成時に_atkを代入
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BulletControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private float _bulletSpeed;//弾が進むスピード
    [SerializeField] private  int _attack;//攻撃力　クローンする時に入れる
    [SerializeField] private float _destroyDistance;
    [SerializeField] private Vector3 _rotationPower;
    [SerializeField] private int _reflectionCount;
    [SerializeField] private Target _ignoreTarget = Target.None;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Gradient _playerTrajectory;
    [SerializeField] private Gradient _enemyTrajectory;
    private Rigidbody _rb;
    private Vector3 _startPosition;
    private Vector3 _forwardDirection;
    private int _frameCounter = 3;
    public enum Target
    {
        None,Player,Enemy,
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPosition = this.transform.position;
        _forwardDirection = this.transform.forward;
        if (_ignoreTarget == Target.None) Debug.LogWarning("弾が無視するタンクを設定していません");
        if(_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }
        if(_ignoreTarget == Target.Player)
        {
            _trailRenderer.colorGradient = _playerTrajectory;
        }
        else if(_ignoreTarget == Target.Enemy)
        {
            _trailRenderer.colorGradient = _enemyTrajectory;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(_frameCounter > 0) _frameCounter--;

        _rb.linearVelocity = _forwardDirection * _bulletSpeed;
        if (Vector3.Distance(this.transform.position, _startPosition) > _destroyDistance)
        {
            Delete();
        }
        _rb.AddTorque(_rotationPower * Time.deltaTime,ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
       
        if(collision.collider.TryGetComponent(out BulletControl bullet) || collision.collider.TryGetComponent(out ItemBase item))
        {
            //弾とアイテムは無視
            return;
        }
        bool isIgnore = false;
        switch (_ignoreTarget)
        {
            case Target.Player:
                if (collision.collider.TryGetComponent<PlayerController>(out var player))
                    isIgnore = true;
                break;

            case Target.Enemy:
                if (collision.collider.TryGetComponent<EnemyBase>(out var enemy))
                    isIgnore = true;
                break;
        }

        if (photonView.IsMine)
        {
            if(!isIgnore && collision.collider.TryGetComponent(out ITank tank))
            {
                //Sound:弾のダメージ 
                CRIAudioManager.SE.Play("SE", "hit");
                collision.collider.gameObject.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All, _attack);
            }

        }
        if (_frameCounter == 0 && collision.collider.gameObject.CompareTag("Wall") && _reflectionCount > 0)   
        {
            Vector3 normal = collision.contacts[0].normal;

            // 反射ベクトル
            _forwardDirection = Vector3.Reflect(_forwardDirection, normal).normalized;

            // Rigidbody に反映
            _rb.angularVelocity = _forwardDirection * _bulletSpeed;
            this.transform.forward = _forwardDirection;
            _reflectionCount--;
            return;
        }
        Delete();

    }
    public void SetBulletData(int attack, Target target)
    {
        _attack = attack;
        _ignoreTarget = target;
    }
    /// <summary>
    /// 生成したのが自分だったら銃弾を消す
    /// </summary>
    private void Delete()
    {
        if (photonView.IsMine && PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
