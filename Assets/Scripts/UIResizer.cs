using MyBox;
using UnityEngine;

public class UIResizer : MonoBehaviour
{
    public RectTransform follow;

    private void Update() => (transform as RectTransform).SetHeight(follow.rect.height);
}
