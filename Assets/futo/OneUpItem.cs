public class OneUpItem : ItemBase
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public override void HitAction(int viewID)
    {
        gameManager.AddLives();
        Delete();
    }
}
