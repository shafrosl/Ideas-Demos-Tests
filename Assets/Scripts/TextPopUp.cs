using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    public float MaxDistance;
    public TextMeshPro TextRenderer;
    public bool IsCounting => textPopUpCountdown > 0;
    private bool isFading;
    private float textPopUpCountdown;
    private CancellationTokenSource ctx = new();
    
    private void Update()
    {
        if (IsCounting) textPopUpCountdown -= Time.deltaTime;
        else if (!IsCounting && !isFading) FadeOut();
    }

    public virtual TextPopUp Instantiate(Vector3 position)
    {
        if (!GameManager.Instance.GameStarted) return null;
        isFading = false;
        TextRenderer.alpha = 1;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.DOShakePosition(0.1f, 0.25f, 3, 10);
        transform.localScale = Vector3.one;
        transform.position = position;
        transform.parent = GameManager.Instance.PoolController.transform;
        transform.LookAt(GameManager.Instance.PlayerController.transform);
        textPopUpCountdown = 0.6f;
        var distance = Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position);
        if (distance >= MaxDistance)
        {
            transform.localScale = new Vector3(1 + (distance / 100), 1 + (distance / 100), 1 + (distance / 100));
        }

        transform.DOLocalMoveY(transform.position.y + 2.5f, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        return this;
    }

    public virtual async void IncreaseSize()
    {
        textPopUpCountdown = 1.25f;
        transform.DOShakePosition(0.1f, 0.25f, 3, 10);
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        await DOTween.To(() => TextRenderer.color, x => TextRenderer.color = x, GameManager.Instance.Red, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        DOTween.To(() => TextRenderer.color, x => TextRenderer.color = x, new Color(0.01176471f, 0.01176471f, 0.01176471f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }

    private async void FadeOut()
    {
        isFading = true;
        await TextRenderer.DOFade(0, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        gameObject.SetActive(false);
    }
}
