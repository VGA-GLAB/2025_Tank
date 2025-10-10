using UnityEngine;
using UnityEngine.InputSystem;

public class RotateTowardsMouse : MonoBehaviour
{
    [SerializeField] private Transform _turret;
    [SerializeField] private Camera _camera;

    private Vector2 _currentMousePosition;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        RotateToMouse();
    }

    /// <summary>
    /// マウスカーソルの位置にタレットを回転させる
    /// </summary>
    private void RotateToMouse()
    {
        _currentMousePosition = Pointer.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(_currentMousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 direction = hitPoint - _turret.position;
            direction.y = 0f;

            if (direction.sqrMagnitude != 0)
            {
                _turret.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}