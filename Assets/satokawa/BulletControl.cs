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
    [SerializeField] public int _attack;//攻撃力　クローンする時に入れる
    [SerializeField] private float _destroyDistance;
    [SerializeField] private Vector3 _rotationPower;
    private Rigidbody _rb;
    private Vector3 _startPosition;
    private Vector3 _forwardDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPosition = this.transform.position;
        _forwardDirection = this.transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _forwardDirection * _bulletSpeed;
        if (Vector3.Distance(this.transform.position, _startPosition) > _destroyDistance)
        {
            Delete();
        }
        _rb.AddTorque(_rotationPower * Time.deltaTime,ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.TryGetComponent(out BulletControl bullet) || collision.TryGetComponent(out ItemBase item))
        {
            //弾とアイテムは無視
            return;
        }
        if (photonView.IsMine)
        {
            if(collision.TryGetComponent(out ITank tank))
            {
                collision.gameObject.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All, _attack);

            }
        }
        Delete();

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
