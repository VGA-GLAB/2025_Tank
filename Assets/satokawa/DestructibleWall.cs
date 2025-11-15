using UnityEngine;
using Photon.Pun;
public class DestructibleWall : MonoBehaviourPunCallbacks, ITank
{
    public int Hp { get; private set; }
    public int AttackPower { get; }
    public float MoveSpeed { get; }

    void Start()
    {
        Hp = 1;
    }
    public void Die()
    {
        //Sound: 壊せる壁壁
        CRIAudioManager.SE.Play("SE", "wall_break_");
        if(photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    [PunRPC]
    public void Hit(int attack)
    {
        Hp -= attack;
        if(Hp <= 0)
        {
            Die();
        }
    }
}
