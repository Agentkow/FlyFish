using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private FishingPlayerCharacterController fpcc;
    [Header("Plug inspector")]
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject miniGameUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider miniGameSlider;
    [SerializeField] private Image hookTicker;
    [SerializeField] private TextMeshProUGUI hookNumText;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform redZone;

    [SerializeField] private MinigameState mState;

    [Header("Values")]
    [SerializeField] private float slideSpeed = 20f;
    [field: SerializeField] public float minTargetRange { get; private set; }
    [field: SerializeField] public float maxTargetRange { get; private set; }
    [field: SerializeField] public float minBreakRange { get; private set; }
    [field: SerializeField] public float maxBreakRange { get; private set; }

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
    public void SetTicker(float value)
    {
        tickerValue = value;
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
    #region Pausing
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

    private bool PauseInput()
    {
        return Input.GetButtonDown("Cancel");
    }
    #endregion
    public void ResetTicker()
    {
        hookTicker.fillAmount = 0;
        canHook = false;
    }

    #region Minigame Slider

    public void StartFishingMinigame(FishStats fs)
    {
        SetUpMinigame(fs);
        miniGameUI.gameObject.SetActive(true);
        mState = MinigameState.Left;
    }

    void SetUpMinigame(FishStats fs)
    {
        for (int i = 0; i < fs.strikeRange.Count; i++)
        {
            greenZone.anchoredPosition = new Vector3(fs.strikeRangeVisual[i].x,1,0);
            greenZone.sizeDelta = new Vector2(fs.strikeRangeVisual[i].y, greenZone.sizeDelta.y);
            minTargetRange = fs.strikeRange[i].x;
            maxTargetRange = fs.strikeRange[i].y;

            redZone.anchoredPosition = new Vector3(fs.breakRangeVisual[i].x, 1, 0);
            redZone.sizeDelta = new Vector2(fs.breakRangeVisual[i].y, redZone.sizeDelta.y);
            minBreakRange = fs.breakRange[i].x;
            maxBreakRange = fs.breakRange[i].y;
        }
        slideSpeed = fs.sliderSpeed;
        
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

    #region Go back to Hub
    public void ReturnToMenu()
    {
        GameManager.gm.GoToHub();
    }
    public void RetreatToMenu()
    {
        GameManager.gm.currentInv.ClearBackpack();
        GameManager.gm.GoToHub();
    }
    #endregion

    public void UpdateHookNum(int hookNum)
    {
        if (hookNum > 0)
            hookNumText.text = hookNum.ToString();
        else
            hookNumText.text = "Out";
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
