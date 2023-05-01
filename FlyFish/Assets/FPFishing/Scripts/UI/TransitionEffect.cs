using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class TransitionEffect : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private UnityEvent<float> TransitionEvent;

    [SerializeField] private float time = 1f;
    public void LoadLevel(string sceneIndex)
    {
        StartCoroutine(Load(sceneIndex));
    }

    IEnumerator Load(string levelIndex)
    {
        Time.timeScale = 1;
        transition.SetTrigger("Start");
        AudioManager.am.SetVolDown();
        yield return new WaitForSecondsRealtime(time);
        GameManager.gm.SceneChange(levelIndex);

    }
}
