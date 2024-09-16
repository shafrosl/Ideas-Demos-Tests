using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public virtual UniTask Initialize() => UniTask.CompletedTask;
}
