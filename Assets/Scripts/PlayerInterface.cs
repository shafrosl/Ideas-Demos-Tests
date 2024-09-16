public interface IMovement
{
    public void Look();
    public void Hide();
}

public interface IGun
{
    public void OnShoot();
    public void OnReload();
}
