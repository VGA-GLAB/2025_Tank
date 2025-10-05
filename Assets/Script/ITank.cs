public interface ITank
{
    public int Hp { get; }
    public int ATK { get; }
    public int MoveSpeed { get; }
    public int BulletInterval { get; }
    public void Hit(int atk);
    public void Die();
}