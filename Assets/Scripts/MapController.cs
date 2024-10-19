using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeReference] private GameObject Range;
    [SerializeReference] private GameObject TimeCrisis;

    public void SetMap()
    {
        if (Range) Range.SetActive(GameManager.Instance.GameMode == GameMode.GunRange);
        if (TimeCrisis) TimeCrisis.SetActive(GameManager.Instance.GameMode == GameMode.TimeCrisis);
    }
}
