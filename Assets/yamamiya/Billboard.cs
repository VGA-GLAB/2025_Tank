using UnityEngine;


public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        this.transform.LookAt(_camera.transform);
    }
}
