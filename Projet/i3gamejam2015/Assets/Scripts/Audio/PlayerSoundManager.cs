using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;

    void Start()
    {
        statusProvider.OnDieAction += onPlayerDied;
    }

    private void onPlayerDied(Vector2 deathVector)
    {
        //SoundManager.Instance.esaioghdsighrjeosri();
    }

    //void Update() { }
}
