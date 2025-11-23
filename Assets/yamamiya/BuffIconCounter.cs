using TMPro;
using UnityEngine;

public class BuffIconCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buffCountText;
    private int _buffCount = 0;

    public void IncrementBuffCount()
    {
        _buffCount++;
        UpdateBuffCountText();
    }

    private void UpdateBuffCountText()
    {
        _buffCountText.text = _buffCount.ToString();
    }
}
