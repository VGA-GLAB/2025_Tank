using Photon.Pun;
using UnityEngine;

public class ChildRotationSync : MonoBehaviour, IPunObservable
{
    [SerializeField] private Transform rotateTarget; // 回転したい子オブジェクト（自分自身でも可）
    [SerializeField] private float smooth = 10f;     // 補間速度

    private PhotonView pv;
    private Quaternion networkRotation;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (rotateTarget == null) rotateTarget = transform;
        networkRotation = rotateTarget.localRotation;
    }

    void Update()
    {
        // 所有者（自分）は回転処理を自前で行うので補間しない
        if (pv.IsMine) return;

        // 他プレイヤーの回転を補間で反映
        rotateTarget.localRotation = Quaternion.Lerp(
            rotateTarget.localRotation,
            networkRotation,
            Time.deltaTime * smooth
        );
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 所有者だけ送信
            stream.SendNext(rotateTarget.localRotation);
        }
        else
        {
            // 非所有者は受信
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
