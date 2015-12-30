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
		AudioSingleton<VoiceAudioManager>.Instance.SelectCharacterPlay();
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
		AudioSingleton<MusicAudioManager>.Instance.SetDefaultSnapshot();
    }
    public void Stage_Pause()
    {
		//To Do apply EQ
		AudioSingleton<MusicAudioManager>.Instance.SetDefaultSnapshot();
    }
    public void Stage_Unpause()
    {
        //TODO
        audioSource.UnPause();
    }
    public void Cursor_Play()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
    }
    public void Validate_Play()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayValidate();
    }
    public void Cancel_Play()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayCancel();
    }

    public void VOICE_Title_Play()
    {
		AudioSingleton<VoiceAudioManager>.Instance.PlayTitle();
    }

    public void GAMEPLAY_Victory()
    {
		//stop the music
		AudioSingleton<MusicAudioManager>.Instance.SetDefaultSnapshot();
		//play sound
		AudioSingleton<SfxAudioManager>.Instance.PlayVictoryJingle();
    }
    public void GAMEPLAY_Jump()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayJump();
    }
    public void GAMEPLAY_Land()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayLand();
    }
    public void GAMEPLAY_Walljump()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayWallJump();
    }
    public void GAMEPLAY_Attack()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
    }
    public void GAMEPLAY_Death()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayDeath();
    }
    public void GAMEPLAY_Rebirth()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayReBirth();
    }
    public void GAMEPLAY_Knockback()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayKnockBack();
    }
    public void GAMEPLAY_Ready()
    {
		AudioSingleton<VoiceAudioManager>.Instance.PlayGameReady();
    }
    public void GAMEPLAY_Fight()
    {
		AudioSingleton<VoiceAudioManager>.Instance.PlayFight();
    }
    public void GAMEPLAY_Gameover()
    {
		AudioSingleton<VoiceAudioManager>.Instance.PlayGameOver();
    }

    public void StartBound()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayStartBound();
    }

    public void StopBound()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayStopBound();
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
		AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
    }
    
    public void TriggerGameFinished()
    {
		AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
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