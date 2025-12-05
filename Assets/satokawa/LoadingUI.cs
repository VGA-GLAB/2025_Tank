using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;
    public void Awake()
    {
        Instance = this;
    }

    public void ShowLoading(string message)
    {
        _text.text = message;
        _panel.SetActive(true);
    }
    public void HideLoading()
    {
        _panel.SetActive(false);
    }
}
