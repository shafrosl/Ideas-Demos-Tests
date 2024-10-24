using System.Linq;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleSystem[] ParticlesSystems;

    public void Play()
    {
        foreach (var particle in ParticlesSystems) particle.Play();
    }

    public void Stop()
    {
        foreach (var particle in ParticlesSystems) particle.Stop();
    }

    public bool IsStopped()
    {
        return ParticlesSystems.All(particle => particle.isStopped);
    }
    
    public bool IsPlaying()
    {
        return ParticlesSystems.All(particle => particle.isPlaying);
    }
}
