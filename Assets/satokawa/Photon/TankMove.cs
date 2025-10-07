using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
///　TODO: テスト用なのでいつか消す
/// </summary>
public class TankMove : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"]; // InputAction の名前
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float y = input.y;
        float x = input.x;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(this.transform.forward * y * Time.deltaTime * 400, ForceMode.Impulse);
        transform.Rotate(0f, x  * Time.deltaTime * 250, 0f);

    }
}
