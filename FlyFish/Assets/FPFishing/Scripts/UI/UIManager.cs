using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private FishingPlayerCharacterController fpcc;
    [Header("Plug inspector")]
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject miniGameUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider miniGameSlider;
    [SerializeField] private Image hookTicker;
    
    [SerializeField] private MinigameState mState;

    [Header("Values")]
    [SerializeField] private float slideSpeed = 20f;
    [field: SerializeField] public float minTargetRange { get; private set; }
    [field: SerializeField] public float maxTargetRange { get; private set; }
    [field: SerializeField] public float sliderVal { get; private set; }
    [field: SerializeField] public float tickerValue { get; private set; }
    [field: SerializeField] public bool canHook { get; private set; }

    public enum MinigameState
    {
        Base,
        Pause,
        Left,
        Right
    }

    private void Start()
    {
        mState = MinigameState.Base;
        Cursor.lockState = CursorLockMode.Locked;
        fpcc = GameObject.Find("Player").GetComponent<FishingPlayerCharacterController>();
        canHook = true;
    }
    // Update is called once per frame
    void Update()
    {
        switch (fpcc.state)
        {
            case FishingPlayerCharacterController.PlayerState.Catch:
                MinigameRun();
                sliderVal = miniGameSlider.value;
                hookTicker.fillAmount += tickerValue * Time.fixedDeltaTime;
                if (hookTicker.fillAmount >= 1 && !canHook)
                    canHook = true;
                break;
            default:
                break;
        }

        if (PauseInput())
        {
            if (Time.timeScale != 0)
                Pause();

        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    public void UnPause()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void ResetTicker()
    {
        hookTicker.fillAmount = 0;
        canHook = false;
    }

    #region Minigame Slider
    public void StartFishingMinigame()
    {
        miniGameUI.gameObject.SetActive(true);
        mState = MinigameState.Left;
    }

    public void EndFishingMinigame()
    {
        miniGameSlider.value = 0;
        mState = MinigameState.Base;
        miniGameUI.gameObject.SetActive(false);
    }

    private void MinigameRun()
    {
        switch (mState)
        {
            case MinigameState.Left:
                miniGameSlider.value += slideSpeed * Time.deltaTime;
                if (miniGameSlider.value>=miniGameSlider.maxValue)
                    mState = MinigameState.Right;
                break;
            case MinigameState.Right:
                miniGameSlider.value -= slideSpeed * Time.deltaTime;
                if (miniGameSlider.value <= miniGameSlider.minValue)
                    mState = MinigameState.Left;
                break;
        }
    }
    #endregion

    private bool PauseInput()
    {
        return Input.GetButtonDown("Cancel");
    }
}
