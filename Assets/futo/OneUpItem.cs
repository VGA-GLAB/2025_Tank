public class OneUpItem : ItemBase
{
    GameManager gameManager;

    public override void HitAction(int viewID)
    {
        gameManager.AddLives();
        Delete();
    }
}
