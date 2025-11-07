using Photon.Pun;
using UnityEngine;

public class ChildRotationSync : MonoBehaviour, IPunObservable
{
    [SerializeField] private Transform _rotateTarget; // 回転したい子オブジェクト（自分自身でも可）
    [SerializeField] private float _smooth = 10f;     // 補間速度

    private PhotonView pv;
    private Quaternion networkRotation;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (_rotateTarget == null) _rotateTarget = transform;
        networkRotation = _rotateTarget.localRotation;
    }

    void Update()
    {
        // 所有者（自分）は回転処理を自前で行うので補間しない
        if (pv.IsMine) return;

        // 他プレイヤーの回転を補間で反映
        _rotateTarget.localRotation = Quaternion.Lerp(
            _rotateTarget.localRotation,
            networkRotation,
            Time.deltaTime * _smooth
        );
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 所有者だけ送信
            stream.SendNext(_rotateTarget.localRotation);
        }
        else
        {
            // 非所有者は受信
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
