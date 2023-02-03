using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [field: SerializeField] public GameState state { get; private set; }
    public enum GameState
    {
        Menu,
        Tutorial,
        InGame
    }

    private void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("GameController");
        if (obj.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Menu;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.Menu:
                break;
            case GameState.Tutorial:
                break;
            case GameState.InGame:
                break;
        }
    }

    public void GoToGame()
    {
        state = GameState.InGame;
    }

    public void ReturnToMenu()
    {
        state = GameState.Menu;
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
