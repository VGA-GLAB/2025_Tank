using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
public static class CustomPropertiesManager
{
    private static Hashtable propsToSet = new();
    /// <summary>
    /// CustomPropertiesにkey,valueをセットする
    /// </summary>
    /// <param name="player">実行するプレイヤー</param>
    /// <param name="key">変数名</param>
    /// <param name="value">値</param>
    public static void SetNetValue(this Player player, string key, float value)
    {
        propsToSet[key] = value;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
    /// <summary>
    /// CustomPropertiesからkeyに対応するfloatを取得する
    /// </summary>
    /// <param name="player">実行するプレイヤー</param>
    /// <param name="key">変数名</param>
    /// <param name="found">変数名に対応するCustomPropertiesが見つかったか ture 見つかった　false　見つからなかった</param>
    /// <returns>keyに対応する値</returns>
    public static float GetNetValue(this Player player, string key,out bool found)
    {
        if (player.CustomProperties.TryGetValue(key, out object propValue))
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
}
