using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utility;

public class DataObject : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI Name;
    
    public void Initialize(Modifier modifier, int value)
    {
        Slider.Initialize(modifier, value);
        Name.text = modifier.GetName();
    }

    public async UniTask<UniTask> Fade(int value, float duration = 0)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        await UniTask.WhenAll(
        Name.DOFade(value, duration).WithCancellation(ct),
        Slider.Main.DOFade(value, duration).WithCancellation(ct),
        Slider.Decrement.DOFade(value > 0 ? 0.75f : 0, duration).WithCancellation(ct),
        Slider.Increment.DOFade(value > 0 ? 0.75f : 0, duration).WithCancellation(ct),
        Slider.Border.DOFade(value, duration).WithCancellation(ct)
        );
        
        return UniTask.CompletedTask;
    }
}
