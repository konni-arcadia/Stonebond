using UnityEngine;
using System.Collections;

public class UnpausableParticleSystem : MonoBehaviour {

    private ParticleSystem particles;

    private bool wasPaused = false;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void Update () 
    {
        if (TimeManager.isPaused)
        {
            particles.Simulate(TimeManager.realDeltaTime, true, false);
            wasPaused = true;
        }
        else if(wasPaused)
        {
            particles.Play();
            wasPaused = false;
        }
    }
}
