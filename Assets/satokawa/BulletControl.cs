using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BulletControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private float _bulletSpeed;//弾が進むスピード
    [SerializeField] public int _atk;//攻撃力　クローンする時に入れる
    [SerializeField] private float destroyDistance;
    private Rigidbody rb;
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = this.transform.forward * _bulletSpeed;
        if (Vector3.Distance(this.transform.position, startPosition) > destroyDistance)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine )
        {
            return;
        }
        ITank tank = collision.gameObject.GetComponent<ITank>();
        if (tank != null)
            collision.gameObject.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All,_atk);

        PhotonNetwork.Destroy(this.gameObject);
    }

}
