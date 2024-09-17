using Cysharp.Threading.Tasks;
using UnityEngine;

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

public interface IBulletHole
{
    public UniTask InstantiateHole(Vector3 position);
}
