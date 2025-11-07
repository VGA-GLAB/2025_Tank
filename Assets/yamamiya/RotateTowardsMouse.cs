using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateTowardsMouse : MonoBehaviourPunCallbacks
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
        if (photonView.IsMine)
        {
            RotateToMouse();
        }
    }

    /// <summary>
    /// マウスカーソルの位置にタレットを回転させる
    /// </summary>
    private void RotateToMouse()
    {
        _currentMousePosition = Pointer.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(_currentMousePosition);

        // タレットの位置を通る水平面を作る
        Plane groundPlane = new Plane(Vector3.up, _turret.position);

        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            // ローカル空間に変換
            Vector3 localHit = _turret.parent.InverseTransformPoint(hitPoint);
            Vector3 localDir = localHit - _turret.localPosition;

            // 水平距離と高さを求めてピッチ角を計算
            float horizontalDistance = new Vector2(localDir.z, localDir.y).magnitude;
            if (horizontalDistance > 0.001f)
            {
                float angleX = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

                // X軸だけ回転
                _turret.localRotation = Quaternion.Euler(angleX, 0f, 0f);
            }
        }




    }
}