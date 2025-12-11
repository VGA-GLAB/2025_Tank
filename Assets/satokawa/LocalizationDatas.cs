using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LocalizationDatas")]
public class LocalizationDatas : ScriptableObject
{
    [Header("===ログ===")]
    [Space(10)]
    [Header("切断中...")]
    public LocalizedString Disconnect;
    [Header("ゲームを開始します...")]
    public LocalizedString StartGame;
    [Header("サーバーに接続中...")]
    public LocalizedString ConnectingToServer;
    [Header("ロビーに接続中...")]
    public LocalizedString ConnectingToLobby;
    [Header("ルーム作成中...")]
    public LocalizedString CreatingRoom;
    [Header("ルーム接続中...")]
    public LocalizedString RoomConnecting;

    [Space(20)]

    [Header("===ルーム命名エラー===")]
    [Space(10)]
    [Header("1文字以上にしてください")]
    public LocalizedString TooShortText;
    [Header("10文字以下にしてください")]
    public LocalizedString TooLongText;
    [Header("スペースを含めることはできません")]
    public LocalizedString ContainsSpaces;
    [Header("/ \\ は使えません")]
    public LocalizedString NotAvailableSymbol;
    [Header("このルーム名はすでに使用されています")]
    public LocalizedString UsedNaming;

    [Space(20)]

    [Header("===接続エラー===")]
    [Space(10)]
    [Header("サーバーに接続できません。\nネットワークやアドレスを確認してください。")]
    public LocalizedString ExceptionOnConnect;
    [Header("接続中にエラーが発生しました。")]
    public LocalizedString Exception;
    [Header("サーバーが応答しません。\n時間をおいて再接続してください。")]
    public LocalizedString ServerTimeout;
    [Header("サーバーの応答が遅すぎます。\n再接続を試みてください。")]
    public LocalizedString ClientTimeout;
    [Header("サーバーにより切断されました。")]
    public LocalizedString DisconnectByServerReasonUnknown;
    [Header("アプリIDが無効です。\n開発者にお問い合わせください。")]
    public LocalizedString InvalidAuthentication;
    [Header("認証に失敗しました。")]
    public LocalizedString CustomAuthenticationFailed;
    [Header("認証が期限切れです。\n再度ログインしてください。")]
    public LocalizedString AuthenticationTicketExpired;
    [Header("同時接続数が上限に達しています。\n後で再試行してください。")]
    public LocalizedString MaxCcuReached;
    [Header("この地域のサーバーに接続できません。\n別の地域でお試しください。")]
    public LocalizedString InvalidRegion;
    [Header("エラーが発生しました。\n再試行してください。")]
    public LocalizedString OperationNotAllowedInCurrentState;
    [Header("不明なエラーが発生しました。")]
    public LocalizedString DefaultError;
    [Header("インターネットに接続されていません。")]
    public LocalizedString NoInternetConnection;

    [Header("===ゲーム内エラー===")]
    [Header("通信環境が悪いため、ルームから退出します")]
    public LocalizedString LowNetworkSpeed;
    [Header("インターネットから切断されました。ルームから退出します")]
    public LocalizedString InternetDisconnect;
}