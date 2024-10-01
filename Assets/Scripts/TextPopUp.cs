using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Debug = Utility.Debug;

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
        textPopUpCountdown = 2.5f;
        var distance = Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position);
        Debug.Log("how far? " + distance);
        if (distance > MaxDistance)
        {
            transform.localScale = new Vector3(1 + (distance / 100), 1 + (distance / 100), 1 + (distance / 100));
        }

        transform.DOLocalMoveY(transform.position.y + 2.5f, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        return this;
    }

    public virtual void IncreaseSize()
    {
        transform.DOShakePosition(0.1f, 0.25f, 3, 10);
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        textPopUpCountdown = 2.5f;
    }

    private async void FadeOut()
    {
        isFading = true;
        await TextRenderer.DOFade(0, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        gameObject.SetActive(false);
    }
}
