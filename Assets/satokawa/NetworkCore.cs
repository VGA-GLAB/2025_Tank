using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Linq;
public static class NetworkCore
{
    private static Hashtable _propsToSet = new();
    /// <summary>
    /// CustomPropertiesにkey,valueをセットする
    /// </summary>
    /// <param name="player">実行するプレイヤー</param>
    /// <param name="key">変数名</param>
    /// <param name="value">値</param>
    public static void SetNetValue(string key, float value)
    {
        _propsToSet[key] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(_propsToSet);
        _propsToSet.Clear();
    }
    /// <summary>
    /// CustomPropertiesからkeyに対応するfloatを取得する
    /// </summary>
    /// <param name="player">実行するプレイヤー</param>
    /// <param name="key">変数名</param>
    /// <param name="found">変数名に対応するCustomPropertiesが見つかったか ture 見つかった　false　見つからなかった</param>
    /// <returns>keyに対応する値</returns>
    public static float GetNetValue(string key,out bool found)
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            found = false;
            return 0f;
        }
        
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object propValue))
        {
            if (propValue is float floatValue)
            {
                found = true;
                return floatValue;
            }
            if (propValue is double doubleValue)
            {
                found = true;
                return (float)doubleValue;
            }
        }
        found = false;
        return 0f;
    }
    /// <summary>
    ///アクターナンバーの欠番を無くしたPlayerNumberを返す
    /// </summary>
    /// <param name="player">取得したいPlayer</param>
    /// <returns>PlayerNumber</returns>
    public static int GetPlayerNumber(Player player)
    {
        if(player == null)
        {
            Debug.LogError("PlayerNull");
            return -1;
        }

        Player[] players = PhotonNetwork.PlayerList;

        players = players.OrderBy(p => p.ActorNumber).ToArray();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ActorNumber == player.ActorNumber)
            {
                return i + 1;
            }
        }
        return -1;
    }
}
