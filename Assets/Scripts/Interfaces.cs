using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IMovement
{
    public void Look();
    public void Hide();
    public void Move();
}

public interface IGun
{
    public void OnShoot();
    public void OnReload();
}

public interface IScore
{
    public void OnHit();
}

public interface IBulletHole
{
    public UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR);
}