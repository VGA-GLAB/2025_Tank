using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class BulletControl : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;//弾が進むスピード
    [SerializeField] public int _atk;//攻撃力　クローンする時に入れる
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(this.transform.forward * _bulletSpeed * Time.deltaTime,ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ITank tank = collision.gameObject.GetComponent<ITank>();
        if (tank != null)
            tank.Hit(_atk);
    }

}
