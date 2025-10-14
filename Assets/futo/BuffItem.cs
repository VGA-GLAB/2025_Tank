using UnityEngine;

public class BuffItem : ItemBase
{
    [SerializeField] Buff _toBuff;
    [SerializeField] float _buffAmount;

    public override void HitAction(GameObject hitObject)
    {
        if(hitObject.TryGetComponent(out PlayerController target))
        {
            target.BuffStatus(_toBuff, _buffAmount);
        }
        else
        {
            return;
        }
        Delete();
    }
}
