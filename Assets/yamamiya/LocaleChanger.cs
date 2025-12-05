using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleChanger : MonoBehaviour
{
    /// <summary>
    /// 指定し言語コードに切り替える
    /// </summary>
    /// <param name="locale"></param>
    public void Chanage(string locale)
    {
        var _ = ChangeSelectedLocale(locale);
    }


    /// <summary>
    /// 指定されたロケール（言語コード）に切り替える
    /// </summary>
    /// <param name="locale"></param>
    /// <returns></returns>
    private async UniTask ChangeSelectedLocale(string locale)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        Locale target = null;

        foreach(var loc in locales)
        {
            // 指定された言語コードと一致するロケールを探す
            if (loc.Identifier.Code == locale)
            {
                target = loc;
                break;
            }
        }

        if(target == null)
        {
            Debug.LogWarning($"指定されたロケールが見つかりません: {locale}");
            return;
        }

        // ロケールを切り替え
        LocalizationSettings.SelectedLocale = target;

        await LocalizationSettings.InitializationOperation.Task;
    }
}
