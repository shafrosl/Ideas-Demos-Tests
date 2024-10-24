using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    [SerializeReference] private GameObject TextPopUp;
    [SerializeReference] private GameObject Spark;
    private List<TextPopUp> TextPopUps = new();
    private List<Particle> Sparks = new();

    public TextPopUp InstantiateHit(Vector3 position)
    {
        foreach (var pop in TextPopUps.Where(pop => !pop.gameObject.activeSelf))
        {
            pop.Instantiate(position);
            return pop;
        }
        
        var p = Instantiate(TextPopUp).GetComponent<TextPopUp>().Instantiate(position);
        TextPopUps.Add(p);
        return p;
    }
    
    public Particle InstantiateSparks(Vector3 position, Quaternion rotation)
    {
        if (!GameManager.Instance.GameStarted) return null;
        foreach (var particle in Sparks.Where(part => part.IsStopped()))
        {
            SetPSPosition(particle, position, rotation);
            return particle;
        }
        
        var p = Instantiate(Spark).GetComponent<Particle>();
        SetPSPosition(p, position, rotation);
        Sparks.Add(p);
        p.Play();
        return p;
    }
    
    public void SetPSPosition(Particle particle, Vector3 position, Quaternion rotation)
    {
        if (!particle.gameObject.activeSelf) particle.gameObject.SetActive(true);
        particle.transform.localScale = Vector3.one;
        particle.transform.position = position;
        particle.transform.rotation = rotation;
        particle.transform.parent = GameManager.Instance.PoolController.transform;
        particle.Play();
    }
}
