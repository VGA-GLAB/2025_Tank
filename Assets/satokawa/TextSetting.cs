//=====  全部 Chat-GPT =====
#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Components;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

public class TextSetting : MonoBehaviour
{

}
[CustomEditor(typeof(TextSetting))]
public class TextSettingEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextSetting script = (TextSetting)target;
        GameObject go = script.gameObject;

        if (GUILayout.Button("設定"))
        {
            SetupPersistentListener(go);
        }
    }

    private void SetupPersistentListener(GameObject go)
    {
        // TextMeshProUGUI を確保（無ければ追加）
        var text = go.GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            text = go.AddComponent<TextMeshProUGUI>();
            Debug.Log("[TestSettingEditor] TextMeshProUGUI added.");
            EditorUtility.SetDirty(go);
        }

        // LocalizeStringEvent を確保（無ければ追加）
        var locEvent = go.GetComponent<LocalizeStringEvent>();
        if (locEvent == null)
        {
            locEvent = go.AddComponent<LocalizeStringEvent>();
            Debug.Log("[TestSettingEditor] LocalizeStringEvent added.");
            EditorUtility.SetDirty(go);
        }

        // 既に永続リスナーとして登録されているかチェック
        bool already = false;
        int persistentCount = locEvent.OnUpdateString.GetPersistentEventCount();
        for (int i = 0; i < persistentCount; i++)
        {
            Object targetObj = locEvent.OnUpdateString.GetPersistentTarget(i) as Object;
            string methodName = locEvent.OnUpdateString.GetPersistentMethodName(i);
            if (targetObj == (Object)text && methodName == "SetText")
            {
                already = true;
                break;
            }
        }

        if (!already)
        {
            // TMP_Text.SetText(string) を直接永続リスナーとして追加
            UnityAction<string> action = text.SetText;
            UnityEventTools.AddPersistentListener(locEvent.OnUpdateString, action);

            // (補助) 引数が string の SetText を選んでいるはずだが、
            // Inspector に表示されているか必ず確認してください。

            // 変更を保存対象にする
            EditorUtility.SetDirty(locEvent);
            EditorUtility.SetDirty(go);
            EditorSceneManager.MarkSceneDirty(go.scene);

            Debug.Log("[TestSettingEditor] Persistent listener (TextMeshProUGUI.SetText) added to OnUpdateString.");
        }
        else
        {
            // すでに永続リスナーがある
            // Debug.Log("[TestSettingEditor] Persistent listener already exists.");
        }
        DestroyImmediate(target as TextSetting, true); // コンポーネント削除
    }
}
#endif