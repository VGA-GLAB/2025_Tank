using System.Collections.Generic;
using UnityEngine;

public class BuffUIManager : MonoBehaviour
{
    [SerializeField] private BuffIconEntry[] _buffIconPrefabs;
    [SerializeField] private GameObject _buffIconParent;

    private Dictionary<Buff, GameObject> _buffPrefabs = new Dictionary<Buff, GameObject>();
    private Dictionary<Buff, BuffIconCounter> _buffCountDictionary = new Dictionary<Buff, BuffIconCounter>();

    [System.Serializable]
    public class BuffIconEntry
    {
        public Buff Buff;
        public GameObject Prefab;
    }

    void Awake()
    {
        foreach (var entry in _buffIconPrefabs)
        {
            _buffPrefabs.Add(entry.Buff, entry.Prefab);
        }
    }

    /// <summary>
    /// バフをUIに追加します。または、既存のバフのカウントを増やします。
    /// </summary>
    /// <param name="buff"></param>
    public void AddBuff(Buff buff)
    {
        // すでにバフが存在する場合はカウントを増やす
        if (_buffCountDictionary.TryGetValue(buff, out BuffIconCounter counter))
        {
            counter.IncrementBuffCount();
            return;
        }

        CreateBuffIcon(buff);
    }

    /// <summary>
    /// バフアイコンのUIを生成します。
    /// </summary>
    /// <param name="buff"></param>
    private void CreateBuffIcon(Buff buff)
    {
        if (_buffPrefabs.TryGetValue(buff, out GameObject buffItem))
        {
            var newBuffIcon = Instantiate(buffItem, _buffIconParent.transform, false);
            if (newBuffIcon.TryGetComponent(out BuffIconCounter buffCount))
            {
                buffCount.IncrementBuffCount();
                _buffCountDictionary.Add(buff, buffCount);
            }
        }
    }
}
