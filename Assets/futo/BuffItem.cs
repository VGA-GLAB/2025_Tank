using UnityEngine;

public class BuffItem : ItemBase
{
    [SerializeField] Buff _toBuff;
    [SerializeField] float _buffamount;

    //ItemDataBaseのonTriggerEnterで取得してほしい
    PlayerController _target;
    public override void HitAction()
    {
        _target.BuffStatus(_toBuff, _buffamount);
        Delete();
    }
}
