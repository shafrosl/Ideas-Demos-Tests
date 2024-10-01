using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    [SerializeReference] private GameObject TextPopUp;
    private List<TextPopUp> TextPopUps = new();

    public TextPopUp InstantiateHit(Vector3 position)
    {
        if (position.y < 5.5f) position = new Vector3(position.x, 2.5f, position.z);
        foreach (var pop in TextPopUps.Where(pop => !pop.gameObject.activeSelf))
        {
            pop.Instantiate(position);
            return pop;
        }
        
        var p = Instantiate(TextPopUp).GetComponent<TextPopUp>().Instantiate(position);
        TextPopUps.Add(p);
        return p;
    }
}
