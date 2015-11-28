﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public float BPM = 110;
    private float quarterNote;
    private float transitionIn;
    private float transitionOut;
    private AudioMixer mainMixer;

    private AudioSource audioSource;
    public AudioClip PressStartScreen;
    public AudioClip CharacterSelect;
    public AudioClip StageSelect;
    public AudioClip StageSpireHigh;
    public AudioClip StagePipesOfAwakening;
    public AudioClip StageCloisterOfTheSilence;
    public AudioClip StageRosetteOfTheWingedOnes;
    public AudioClip Cursor;
    public AudioClip Validate;
    public AudioClip Cancel;
    public AudioClip VOICETitle;
    public AudioClip VOICECharacterSelect;
    public AudioClip VOICEGetReady;
    public AudioClip VOICEFight;
    public AudioClip VOICEGameover;
    public AudioClip BoundStart;
    public AudioClip BoundBreak;
    public AudioClip VictoryJingle;

    public AudioClip Jump;
    public AudioClip WallJump;
    public AudioClip AttackA;
    public AudioClip AttackB;
    public AudioClip Knockback;
    public AudioClip Land;
    public AudioClip Death;
    public AudioClip Rebirth;


    //public AudioClip SelectYourCharacter;
    //public AudioClip SelectYourStage;
    //public AudioClip Bound;

    public enum StageEnum
    {
        PipesOfAwakening,
        SpireHigh,
        CloisterOfTheSilence,
        RosetteOfTheWingedOnes,
    }

    //public AudioMixerSnapshot outOfCombat;
    //public AudioMixerSnapshot inCombat;
    //public AudioClip CharacterSelect;
    //public AudioClip StageSelect;
    //public AudioSource stingSource;
    //public float bpm = 70;



    #region SINGLETON
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        quarterNote = 60f / BPM;
        transitionIn = quarterNote / 8f;
        transitionOut = quarterNote * 8f;
        mainMixer = Resources.Load<AudioMixer>("Main");
    }

    public void PressStart_Play()
    {
        audioSource.clip = PressStartScreen;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void PressStart_Stop()
    {
        audioSource.Stop();
    }
    public void CharacterSelect_Play()
    {
        audioSource.clip = CharacterSelect;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void CharacterSelect_Stop()
    {
        audioSource.Stop();
    }
    public void StageSelect_Play()
    {
        audioSource.clip = StageSelect;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StageSelect_Stop()
    {
        audioSource.Stop();
    }
    public void Stage_Play(StageEnum _stage)
    {
        var source = GetAudioSource("BackgroundSound");
        switch (_stage)
        {
            case StageEnum.PipesOfAwakening:
                source.clip = StageSpireHigh;
                break;
            case StageEnum.SpireHigh:
                source.clip = StagePipesOfAwakening;
                break;
            case StageEnum.CloisterOfTheSilence:
                source.clip = StageCloisterOfTheSilence;
                break;
            case StageEnum.RosetteOfTheWingedOnes:
                source.clip = StageRosetteOfTheWingedOnes;
                break;
        }
        source.loop = true;
        source.Play();
    }
    public void Stage_Stop()
    {
        audioSource.Stop();
    }
    public void Stage_Pause()
    {
        audioSource.Pause();
    }
    public void Stage_Unpause()
    {
        audioSource.UnPause();
    }
    public void Cursor_Play()
    {
        audioSource.PlayOneShot(Cursor);
    }
    public void Validate_Play()
    {
        audioSource.PlayOneShot(Validate);
    }
    public void Cancel_Play()
    {
        audioSource.PlayOneShot(Cancel);
    }

    public void VOICE_Title_Play()
    {
        audioSource.PlayOneShot(VOICETitle);
    }
    public void VOICE_SelectCharacter_Play()
    {
        audioSource.PlayOneShot(VOICECharacterSelect);
    }
    public void VOICE_GetReady_Play()
    {
        audioSource.PlayOneShot(VOICEGetReady);
    }
    public void VOICE_Fight_Play()
    {
        audioSource.PlayOneShot(VOICEFight);
    }
    public void VOICE_Gameover_Play()
    {
        audioSource.PlayOneShot(VOICEGameover);
    }
    public void GAMEPLAY_Bound_Play()
    {
        audioSource.PlayOneShot(BoundStart);
    }
    public void GAMEPLAY_BoundBreak()
    {
        audioSource.PlayOneShot(BoundBreak);
    }
    public void GAMEPLAY_Victory()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(VictoryJingle);
    }
    //public void GAMEPLAY_Airdash()
    //{
    //    audioSource.PlayOneShot(AirDash);
    //}


    public void GAMEPLAY_Jump()
    {
        audioSource.PlayOneShot(Jump);
    }
    public void GAMEPLAY_Land()
    {
        audioSource.PlayOneShot(Land);
    }
    public void GAMEPLAY_Walljump()
    {
        audioSource.PlayOneShot(WallJump);
    }
    public void GAMEPLAY_AttackA()
    {
        audioSource.PlayOneShot(AttackA);
    }
    public void GAMEPLAY_AttackB()
    {
        audioSource.PlayOneShot(AttackB);
    }
    public void GAMEPLAY_Death()
    {
        audioSource.PlayOneShot(Death);
    }
    public void GAMEPLAY_Rebirth()
    {
        audioSource.PlayOneShot(Rebirth);
    }
    public void GAMEPLAY_Knockback()
    {
        audioSource.PlayOneShot(Knockback);
    }
    //public void SelectYourCharacter_Play()
    //{
    //    audioSource.PlayOneShot(SelectYourCharacter);
    //}
    //public void SelectYourStage_Play()
    //{
    //    audioSource.PlayOneShot(SelectYourStage);
    //}

    protected AudioSource GetAudioSource(string _objectID)
    {
        var gameObject = GameObject.Find(_objectID);
        AudioSource source = null;
        if (gameObject != null)
        {
            source = gameObject.GetComponent<AudioSource>();
        }
        return source;
    }


}