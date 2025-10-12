using Photon.Pun;
public interface ITank
{
    public int Hp { get; }
    public int AttackPower { get; }
    public float MoveSpeed { get; }
    [PunRPC]
    public void Hit(int attack);
    public void Die();
}