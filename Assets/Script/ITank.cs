using Photon.Pun;
public interface ITank
{
    public int Hp { get; }
    public int AttackPower { get; }
    public int MoveSpeed { get; }
    public float BulletInterval { get; }
    [PunRPC]
    public void Hit(int atk);
    public void Die();
}