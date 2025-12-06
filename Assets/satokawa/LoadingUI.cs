using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private LocalizeStringEvent _localization;
    public void Awake()
    {
        Instance = this;
        HideLoading();
    }

    public void ShowLoading(LocalizedString message)
    {
        _localization.StringReference = message;
        //_text.text = message;
        _panel.SetActive(true);
    }
    public void HideLoading()
    {
        _panel.SetActive(false);
    }
}
