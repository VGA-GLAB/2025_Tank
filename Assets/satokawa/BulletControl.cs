using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BulletControl : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;//弾が進むスピード
    [SerializeField] public int _atk;//攻撃力　クローンする時に入れる
    [SerializeField] private float destroyDistance;
    private Rigidbody rb;
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = this.transform.forward * _bulletSpeed;
        if (Vector3.Distance(this.transform.position, startPosition) > destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ITank tank = collision.gameObject.GetComponent<ITank>();
        if (tank != null)
            tank.Hit(_atk);

        Destroy(this.gameObject);
    }

}
