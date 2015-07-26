using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip PressStartScreen;
    public AudioClip CharacterSelect;
    public AudioClip StageSelect;
    public AudioClip Stage1;
    public AudioClip Stage2;
    public AudioClip Stage3;
    public AudioClip Stage4;
    public AudioClip Cursor;
    public AudioClip Validate;
    public AudioClip Cancel;
    public AudioClip VOICETitle;
    public AudioClip VOICECharacterSelect;
    public AudioClip VOICEGetReady;
    public AudioClip VOICEFight;
    public AudioClip VOICEGameover;
    //public AudioClip SelectYourCharacter;
    //public AudioClip SelectYourStage;
    //public AudioClip Bound;

    public enum StageEnum
    {
        One,
        Two,
        Three,
        Four,
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
        switch (_stage)
        {
            case StageEnum.One:
                audioSource.clip = Stage1;
                break;
            case StageEnum.Two:
                audioSource.clip = Stage2;
                break;
            case StageEnum.Three:
                audioSource.clip = Stage3;
                break;
            case StageEnum.Four:
                audioSource.clip = Stage4;
                break;
        }
        audioSource.loop = true;
        audioSource.Play();
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
    //public void SelectYourCharacter_Play()
    //{
    //    audioSource.PlayOneShot(SelectYourCharacter);
    //}
    //public void SelectYourStage_Play()
    //{
    //    audioSource.PlayOneShot(SelectYourStage);
    //}
}