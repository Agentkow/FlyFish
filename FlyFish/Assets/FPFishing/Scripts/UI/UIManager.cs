using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Plug inspector")]
    [SerializeField] private FishingPlayerCharacterController fpcc;
    [SerializeField] private GameObject miniGameUI;
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
        Left,
        Right
    }

    private void Start()
    {
        mState = MinigameState.Left;
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
                break;
            default:
                break;
        }

        hookTicker.fillAmount += tickerValue * Time.fixedDeltaTime;
        if (hookTicker.fillAmount>= 1 && !canHook)
            canHook = true;
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
    }

    public void EndFishingMinigame()
    {
        miniGameSlider.value = 0;
        mState = MinigameState.Left;
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
}
