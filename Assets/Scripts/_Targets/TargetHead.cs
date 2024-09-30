public class TargetHead : Target
{
    public override void AddScore()
    {
        GameManager.Instance.PlayerStats.Score.HeadShots++;
    }
}
