using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
public class ErrorMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private LocalizeStringEvent _localization;
    [SerializeField] private LocalizationDatas _localizationDatas;
    public void ShowMessage(LocalizedString message)
    {
        _localization.StringReference = message;
        _messagePanel.SetActive(true);
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