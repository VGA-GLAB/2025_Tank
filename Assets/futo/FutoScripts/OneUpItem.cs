using Photon.Pun;

public class OneUpItem : ItemBase
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    [PunRPC]
    public override void HitAction(int viewID)
    {
        gameManager.AddLives();
        CRIAudioManager.SE.Play("SE", "itemget");
        Delete();
    }
}
