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
        public AudioClip Rebirth;
        public AudioClip Attack01;

        public void Start()
        {
            playerAudioSource = GetComponent<AudioSource>();
        }

        private void PlayOneShot(AudioClip _clip)
        {
            playerAudioSource.PlayOneShot(_clip);
        }

        public void Jump_Play()
        { PlayOneShot(Jump); }

        public void Land_Play()
        { PlayOneShot(Land); }

        //public void Wallslide_Play()
        //{ PlayOneShot(WallSlide); }

        public void Death_Play()
        { PlayOneShot(Death); }

        public void Bond_Stop()
        { PlayOneShot(Land); }

        public void Rebirth_Play()
        { PlayOneShot(Rebirth); }

        public void Attack_Play()
        { PlayOneShot(Attack01); }
    }
}