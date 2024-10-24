public class TargetObstacle : TargetStatic
{
    public override void OnHit() => TargetController.SpecialisedTask();
}
