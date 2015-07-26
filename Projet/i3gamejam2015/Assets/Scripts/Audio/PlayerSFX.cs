using UnityEngine;
using System.Collections;

namespace Completed
{
    public class PlayerSFX : MonoBehaviour
    {
        private AudioSource playerAudioSource;

        public AudioClip Jump;
        public AudioClip Land;
        public AudioClip WallSlide;
        public AudioClip Death;
        public AudioClip BondStart;
        public AudioClip Bound;
        public AudioClip BondStop;
        public AudioClip Rebirth;

        public void Start()
        {
            playerAudioSource = GetComponent<AudioSource>();
        }

        public void PlayOneShot(AudioClip _clip)
        {
            playerAudioSource.PlayOneShot(_clip);
        }
    }
}