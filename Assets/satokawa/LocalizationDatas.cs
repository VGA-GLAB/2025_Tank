using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LocalizationDatas")]
public class LocalizationDatas : ScriptableObject
{
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
}
