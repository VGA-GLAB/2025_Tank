using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, ITank
{
    [SerializeField] private int _hp; //耐久力
    [SerializeField] private int _atk; //攻撃力
    [SerializeField] private int _moveSpeed; //移動速度
    [SerializeField] private int _bulletInterval; //砲弾の連射インターバル
    [SerializeField] private float _turnSpeed; //回転速度

    public int Hp => _hp;
    public int ATK => _atk;
    public int MoveSpeed => _moveSpeed;
    public int BulletInterval => _bulletInterval;

    private Rigidbody _rigidbody;
    private BulletShoot _tankShoot;
    private Vector2 _moveInput;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _tankShoot = GetComponent<BulletShoot>();
        if(_tankShoot != null)
        {
            _tankShoot.SetBulletInterval(_bulletInterval);
        }
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            var x = _moveInput.x * _turnSpeed * Time.deltaTime;
            var z = _moveInput.y * _moveSpeed * Time.deltaTime;

            _rigidbody.AddForce(this.transform.forward * z, ForceMode.Impulse);

            this.transform.Rotate(0, x, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Die()
    {
        // TODO 追加予定
    }

    public void Hit(int atk)
    {
        // TODO 追加予定
    }
}
