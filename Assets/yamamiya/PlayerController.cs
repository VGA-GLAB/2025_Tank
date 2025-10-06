using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, ITank
{
    [SerializeField] private int _hp; //耐久力
    [SerializeField] private int _atk; //攻撃力
    [SerializeField] private int _moveSpeed; //移動速度
    [SerializeField] private int _bulletInterval; //砲弾の連射インターバル

    public int Hp => _hp;
    public int ATK => _atk;
    public int MoveSpeed => _moveSpeed;
    public int BulletInterval => _bulletInterval;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            var x = _moveInput.x * _moveSpeed * Time.deltaTime;
            var z = _moveInput.y * _moveSpeed * Time.deltaTime;
            Vector3 moveDirection = new Vector3(x, 0, z);

            _rigidbody.AddForce(moveDirection, ForceMode.Impulse);
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * _moveSpeed);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Die()
    {

    }

    public void Hit(int atk)
    {

    }
}
