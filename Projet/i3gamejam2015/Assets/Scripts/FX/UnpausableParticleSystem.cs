using UnityEngine;
using System.Collections;

public class UnpausableParticleSystem : MonoBehaviour {

	// This is needed to speed up particle simulation in the case of forward particles
	// during chest blast. Because we move the end collider very quickly and particles
	// don't have time to spawn quick enough to draw a beam.
	// So for those particles we make that happen by giving a big acceleration.
	public float acceleration = 1.0f;

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
			particles.Simulate(TimeManager.realDeltaTime * acceleration, true, false);
            wasPaused = true;
        }
        else if(wasPaused)
        {
            particles.Play();
            wasPaused = false;
        }
    }
}
