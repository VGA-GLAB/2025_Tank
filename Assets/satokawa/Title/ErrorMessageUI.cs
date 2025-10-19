using UnityEngine;
using TMPro;
using Photon.Realtime;
public class ErrorMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TextMeshProUGUI _messageText;
    public void ShowMessage(string message)
    {
        _messageText.text = message;
        _messagePanel.SetActive(true);
    }
    public void ShowMessage(DisconnectCause cause)
    {
        string userMessage = "";
        switch (cause)
        {
            case DisconnectCause.None:
                userMessage = "";
                break;

            case DisconnectCause.ExceptionOnConnect:
                userMessage = "サーバーに接続できません。\nネットワークやアドレスを確認してください。";
                break;

            case DisconnectCause.Exception:
                userMessage = "接続中にエラーが発生しました。";
                break;

            case DisconnectCause.ServerTimeout:
                userMessage = "サーバーが応答しません。\n時間をおいて再接続してください。";
                break;

            case DisconnectCause.ClientTimeout:
                userMessage = "サーバーの応答が遅すぎます。\n再接続を試みてください。";
                break;

            case DisconnectCause.DisconnectByServerLogic:
                userMessage = "";
                break;

            case DisconnectCause.DisconnectByServerReasonUnknown:
                userMessage = "サーバーにより切断されました。";
                break;

            case DisconnectCause.InvalidAuthentication:
                userMessage = "アプリIDが無効です。\n開発者にお問い合わせください。";
                break;

            case DisconnectCause.CustomAuthenticationFailed:
                userMessage = "認証に失敗しました。\n";
                break;

            case DisconnectCause.AuthenticationTicketExpired:
                userMessage = "認証が期限切れです。\n再度ログインしてください。";
                break;

            case DisconnectCause.MaxCcuReached:
                userMessage = "同時接続数が上限に達しています。\n後で再試行してください。";
                break;

            case DisconnectCause.InvalidRegion:
                userMessage = "この地域のサーバーに接続できません。\n別の地域でお試しください。";
                break;

            case DisconnectCause.OperationNotAllowedInCurrentState:
                userMessage = "エラーが発生しました。\n再試行してください。";
                break;

            case DisconnectCause.DisconnectByClientLogic:
                userMessage = "";
                break;

            default:
                userMessage = "不明なエラーが発生しました。";
                break;
        }
        if(userMessage == "")
        {
            return;
        }
        _messagePanel.SetActive(true);
        _messageText.text = userMessage;
    }

    public void Clause()
    {
        _messagePanel.SetActive(false);
    }
}