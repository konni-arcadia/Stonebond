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
        public AudioClip Attack02;
        public AudioClip BondStart;
        public AudioClip Bond;
        public AudioClip BondBreak;
        public AudioClip Knockback;
        public AudioClip WallJump;


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
        {
            int r = Random.Range(0, 2);
            if (r < 1)
                PlayOneShot(Attack01);
            else
                PlayOneShot(Attack02);
        }

        public void Knockback_Play()
        { PlayOneShot(Knockback); }

        public void WallJump_Play()
        { PlayOneShot(WallJump); }
    }
}