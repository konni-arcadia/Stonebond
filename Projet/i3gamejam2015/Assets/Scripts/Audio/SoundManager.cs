using UnityEngine;
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
    #region AudioClips

    public AudioClip Cursor;
    public AudioClip Validate;
    public AudioClip Cancel;
    public AudioClip VOICETitle;
    public AudioClip VOICECharacterSelect;
    public AudioClip VOICEGetReady;
    public AudioClip VOICEFight;
    public AudioClip VOICEGameover;
    public AudioClip BoundStart;
    public AudioClip BoundLoop;
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
    #endregion

    public enum StageEnum
    {
        PipesOfAwakening,
        SpireHigh,
        CloisterOfTheSilence,
        RosetteOfTheWingedOnes,
    }
    
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

        //preload bound
        //var source = GetAudioSource("BondSound");
        //source.clip = BoundLoop;
        //source.loop = true;
        //source.Play();
    }

    public void PressStart_Play()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
    }
    public void PressStart_Stop()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
    }
    public void CharacterSelect_Play()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetSelectCharacterSnapshot();
		VOICE_SelectCharacter_Play();
    }
    public void CharacterSelect_Stop()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
    }
    public void StageSelect_Play()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetSelectSceneSnapshot();
    }
    public void StageSelect_Stop()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
    }
    public void Stage_Play(StageEnum _stage)
    {
        switch (_stage)
        {
            case StageEnum.PipesOfAwakening:
				AudioSingleton<MusicAudioManager>.Instance.SetPipesOfAwakeningSnapshot();
                break;
            case StageEnum.SpireHigh:
				AudioSingleton<MusicAudioManager>.Instance.SetSpireHighSnapshot();
                break;
			case StageEnum.CloisterOfTheSilence:
				AudioSingleton<MusicAudioManager>.Instance.SetCloisterOfTheSilence();
                break;
			case StageEnum.RosetteOfTheWingedOnes:
				AudioSingleton<MusicAudioManager>.Instance.SetRosetteOfTheWingeSnapshot();
                break;
        }
    }
    public void Stage_Stop()
    {
        //TODO
        audioSource.Stop();
    }
    public void Stage_Pause()
    {
        //TODO
        audioSource.Pause();
    }
    public void Stage_Unpause()
    {
        //TODO
        audioSource.UnPause();
    }
    public void Cursor_Play()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Cursor);
    }
    public void Validate_Play()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Validate);
        //audioSource.PlayOneShot(Validate);
    }
    public void Cancel_Play()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Cancel);
        //audioSource.PlayOneShot(Cancel);
    }

    public void VOICE_Title_Play()
    {
        //TODO
        var source = GetAudioSource("VoiceSound");
        source.PlayOneShot(VOICETitle);
        //audioSource.PlayOneShot(VOICETitle);
    }
    public void VOICE_SelectCharacter_Play()
    {
        var source = GetAudioSource("VoiceSound");
        source.PlayOneShot(VOICECharacterSelect);
        //audioSource.PlayOneShot(VOICECharacterSelect);
    }
    public void GAMEPLAY_Victory()
    {
        //TODO
        audioSource.Stop();
        audioSource.PlayOneShot(VictoryJingle);
    }
    public void GAMEPLAY_Jump()
    {
        var source = GetAudioSource("JumpSound");
        source.PlayOneShot(Jump);
    }
    public void GAMEPLAY_Land()
    {
		var source = GetAudioSource("SFXSound");
		source.PlayOneShot(Land);
    }
    public void GAMEPLAY_Walljump()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(WallJump);
    }
    public void GAMEPLAY_Attack()
    {
        var source = GetAudioSource("AttackSound");
        float r = Random.Range(0, 2);
        if (r > 0.5f)
        {
            source.PlayOneShot(AttackA);
        }
        else
        {
            source.PlayOneShot(AttackB);
        }
    }
    public void GAMEPLAY_Death()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Death);
    }
    public void GAMEPLAY_Rebirth()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Rebirth);
    }
    public void GAMEPLAY_Knockback()
    {
        var source = GetAudioSource("SFXSound");
        source.PlayOneShot(Knockback);
    }
    public void GAMEPLAY_Ready()
    {
        var source = GetAudioSource("VoiceSound");
        source.PlayOneShot(VOICEGetReady);
    }
    public void GAMEPLAY_Fight()
    {
        var source = GetAudioSource("VoiceSound");
        source.PlayOneShot(VOICEFight);
    }
    public void GAMEPLAY_Gameover()
    {
        var source = GetAudioSource("VoiceSound");
        source.PlayOneShot(VOICEGameover);
    }

    public void StartBound()
    {
        var snapshot = mainMixer.FindSnapshot("Bond");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionIn);
        }

        var source = GetAudioSource("SFXSound");
        if (source != null)
        {
            source.PlayOneShot(BoundStart);
        }
    }

    public void StopBound()
    {
        var snapshot = mainMixer.FindSnapshot("Background");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionOut);
        }

        var source = GetAudioSource("SFXSound");
        if (source != null)
        {
            source.PlayOneShot(BoundBreak);
        }
    }

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

    #region Triggers
    //TODO
    //Là c'est un peu deg... > faut changer ces trigger en events
    public void TriggerMenuBack()
    {
        var snapshot = mainMixer.FindSnapshot("MainMenu");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionOut);
        }
    }
    
    public void TriggerGameFinished()
    {
        var snapshot = mainMixer.FindSnapshot("MainMenu");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionOut);
        }
    }

    public void TriggerPause()
    {
        var snapshot = mainMixer.FindSnapshot("Pause");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionOut);
        }
    }
    public void TriggerResume()
    {
        var snapshot = mainMixer.FindSnapshot("Background");
        if (snapshot != null)
        {
            snapshot.TransitionTo(transitionOut);
        }
    }

    #endregion
}