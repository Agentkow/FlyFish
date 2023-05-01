using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _am;
    public static AudioManager am { get { return _am; } }

    [SerializeField] private float setVolume = 1;
    [SerializeField] private float mute;

    [field: SerializeField] public AudioState sound { get; private set; }
    public enum AudioState
    {
        FadeIn,
        FadeOut,
        BaseOn,
        BaseOff
    }

    private void Awake()
    {
        
        if (_am != null && _am != this)
            Destroy(this.gameObject);
        else
            _am = this;

        setVolume = PlayerPrefs.GetFloat("Volume");
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartSound();
    }
    void StartSound()
    {
        sound = AudioState.BaseOff;
    }

    private void Update()
    {
        switch (sound)
        {
            case AudioState.FadeIn:
                TurnUpVolume();
                break;
            case AudioState.FadeOut:
                TurnDownVolume();
                break;
            case AudioState.BaseOn:
                break;
            case AudioState.BaseOff:
                AudioListener.volume = 0;
                sound = AudioState.FadeIn;
                break;
        }
    }

    public void SetVolDown()
    {
        sound = AudioState.FadeOut;
    }
    void TurnDownVolume()
    {
        if (AudioListener.volume > 0)
            AudioListener.volume -= (setVolume/2) * Time.deltaTime;
    }

     void TurnUpVolume()
    {
        if (AudioListener.volume <setVolume)
            AudioListener.volume += (setVolume / 2) * Time.deltaTime;
        else
        {
            AudioListener.volume = setVolume;
            sound = AudioState.BaseOn;
        }
            

    }

    public void SetVolume()
    {

    }


    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
    }
}
