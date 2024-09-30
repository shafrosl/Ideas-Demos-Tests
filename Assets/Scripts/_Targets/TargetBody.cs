public class TargetBody : Target
{
    public override void AddScore()
    {
        GameManager.Instance.PlayerStats.Score.BodyShots++;
    }
}
