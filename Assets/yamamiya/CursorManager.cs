using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _cursorImage;
    private RectTransform _canvasRect;
    private RectTransform _cursorRect;
    private Vector2 _currentMousePosition;

    private void Start()
    {
        DisableDefaultCursor();
        _canvasRect = _canvas.GetComponent<RectTransform>();
        _cursorRect = _cursorImage.GetComponent<RectTransform>();
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect,
            Pointer.current.position.ReadValue(), _canvas.worldCamera, out _currentMousePosition);

        _cursorRect.anchoredPosition = _currentMousePosition;
    }

    /// <summary>
    /// 標準カーソルを有効にし、カスタム照準の無効化
    /// </summary>
    public void EnableDefaultCursor()
    {
        Cursor.visible = true;
        _cursorImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 標準カーソルを無効にし、カスタム照準を有効化
    /// </summary>
    public void DisableDefaultCursor()
    {
        Cursor.visible = false;
        _cursorImage.gameObject.SetActive(true);
    }
}
