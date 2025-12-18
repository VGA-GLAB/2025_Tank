using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Events;
using UnityEngine.UI;
public class ErrorMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private LocalizeStringEvent _localization;
    [SerializeField] private LocalizationDatas _localizationDatas;
    [SerializeField] private CursorManager _cursorManager;
    public static ErrorMessageUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if(_cursorManager == null)
        {
            _cursorManager = FindAnyObjectByType<CursorManager>();
        }
        _messagePanel.SetActive(false);
    }
    public void ShowMessage(LocalizedString message,UnityAction action = null)
    {
        _localization.StringReference = message;
        _messagePanel.SetActive(true);
        _confirmButton?.onClick.RemoveAllListeners();
        if (action != null)
        {
            _confirmButton.onClick.AddListener(action);
        }

        if(_cursorManager != null)
        {
            _cursorManager.EnableDefaultCursor();
        }
    }
    public void ShowMessage(DisconnectCause cause)
    {
        LocalizedString userMessage = null;
        switch (cause)
        {
            case DisconnectCause.None:
                userMessage = null;
                break;

            case DisconnectCause.ExceptionOnConnect:
                userMessage = _localizationDatas.ExceptionOnConnect;
                break;

            case DisconnectCause.Exception:
                userMessage = _localizationDatas.Exception;
                break;

            case DisconnectCause.ServerTimeout:
                userMessage = _localizationDatas.ServerTimeout;
                break;

            case DisconnectCause.ClientTimeout:
                userMessage = _localizationDatas.ClientTimeout;
                break;

            case DisconnectCause.DisconnectByServerLogic:
                userMessage = null;
                break;

            case DisconnectCause.DisconnectByServerReasonUnknown:
                userMessage = _localizationDatas.DisconnectByServerReasonUnknown;
                break;

            case DisconnectCause.InvalidAuthentication:
                userMessage = _localizationDatas.InvalidAuthentication;
                break;

            case DisconnectCause.CustomAuthenticationFailed:
                userMessage = _localizationDatas.CustomAuthenticationFailed;
                break;

            case DisconnectCause.AuthenticationTicketExpired:
                userMessage = _localizationDatas.AuthenticationTicketExpired;
                break;

            case DisconnectCause.MaxCcuReached:
                userMessage = _localizationDatas.MaxCcuReached;
                break;

            case DisconnectCause.InvalidRegion:
                userMessage = _localizationDatas.InvalidRegion;
                break;

            case DisconnectCause.OperationNotAllowedInCurrentState:
                userMessage = _localizationDatas.OperationNotAllowedInCurrentState;
                break;

            case DisconnectCause.DisconnectByClientLogic:
                userMessage = null;
                break;

            default:
                userMessage = _localizationDatas.DefaultError;
                break;
        }
        if (userMessage == null)
        {
            return;
        }
        _messagePanel.SetActive(true);
        _localization.StringReference = userMessage;
    }

    public void Clause()
    {
        _messagePanel.SetActive(false);
    }
}