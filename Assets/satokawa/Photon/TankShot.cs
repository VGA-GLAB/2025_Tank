using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
///　TODO: テスト用なのでいつか消す
/// </summary>
public class TankShot : MonoBehaviour
{
    [SerializeField] private Transform clonePosition;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotInterval;
    private float timer;
    private PlayerInput playerInput;
    private InputAction shootAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Jump"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = shotInterval;
    }

    // Update is called once per frame
    void Update()
    {
        bool isPressed = shootAction.ReadValue<float>() > 0.5f;//|| Input.GetKey(KeyCode.Space);
        timer += Time.deltaTime;
        if (isPressed && timer > shotInterval)
        {
            timer = 0f;
            GameObject newBullet = PhotonNetwork.Instantiate(bulletPrefab.name, clonePosition.transform.position, Quaternion.identity);
            newBullet.transform.forward = this.transform.forward;
        }
    }
}
