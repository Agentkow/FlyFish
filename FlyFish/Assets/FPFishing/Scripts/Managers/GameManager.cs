using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _gm;
    public static GameManager gm { get { return _gm; } }

    /* Game Scenes index:
     * Title Screen - 0
     * Hub - 1
     * Island - 2
    */

    [SerializeField] private UIAnimation creditsAnimation;

    [field: Header("Current Equipment")]
    [field: SerializeField] public Inventory currentInv { get; private set; }
    [field: SerializeField] public HookEquipment currentHook { get; private set; }
    [field: SerializeField] public PoleEquipment currentPole { get; private set; }

    [field: Header("Money")]
    [SerializeField] private int costTake = 0;
    [SerializeField] private int costAdd = 0;

    [field: Header("Save Info")] // hold all save info in use here
    [field: SerializeField] public int money { get; private set; }
    [field: SerializeField] public List<bool> hookBought { get; private set; }
    [field: SerializeField] public List<bool> poleBought { get; private set; }
    [field: SerializeField] public List<bool> licenseBought { get; private set; }

    [field: Header("All Equipment Info")]
    [field: SerializeField] public List<HookEquipment> hooks { get; private set; }
    [field: SerializeField] public List<PoleEquipment> poles { get; private set; }
    [field: SerializeField] public List<Licenses> fishLicense { get; private set; }

    [field: Header("Current State")]
    [field: SerializeField] public GameState state { get; private set; }
    public enum GameState
    {
        Menu,
        Hub,
        Tutorial,
        InGame
    }

    public void GetSaveFile() //gets the save file information from the save script
    {
        SaveAndLoad.save.LoadData();
        hookBought = SaveAndLoad.save.getHooklist();
        poleBought = SaveAndLoad.save.getPolelist();
        licenseBought = SaveAndLoad.save.getLicenselist();
        money = SaveAndLoad.save.getMoney();
    }

    private void Awake()
    {
        if (_gm != null && _gm != this)
            Destroy(this.gameObject);
        else
            _gm = this;

        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Hub")
        {

        }
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
            case GameState.Hub:
                MoneyChanging();
                TestMoneyChange();
                break;
            case GameState.Tutorial:
                break;
            case GameState.InGame:
                break;
            default:
                break;
        }
    }

    #region money change

    void MoneyChanging()
    {
        if (costTake!=0)
        {
            money -= 1;
            costTake -= 1;
        }

        if (costAdd!=0)
        {
            money += 1;
            costAdd -= 1;
        }
    }

    public void MoneyDecrease(int cost) => costTake = cost;
    public void MoneyIncrease(int cost) => costAdd = cost;

    public void SellingAllFish()
    {
        if (currentInv.fishContainer.Count>0)
        {
            int incomingCash = (int)currentInv.SellAllFish();
            MoneyIncrease(incomingCash);
            
        }
        
    }

    void TestMoneyChange()
    {
        if (giveInput())
        {
            MoneyIncrease(Random.Range(100,1000));
        }
    }

    private bool giveInput()
    {
        return Input.GetButtonDown("Test");
    }
    #endregion

    #region state machine change
    public void GoToGame() => state = GameState.InGame;
    public void GoToHub() => state = GameState.Hub;
    public void ReturnToMainMenu() => state = GameState.Menu;
    public void SceneChange(string levelIndex) => SceneManager.LoadScene(levelIndex);
    #endregion

    #region Credits
    public void ShowCredits()
    {
        creditsAnimation.Open();
    }

    public void CloseCredits()
    {
        creditsAnimation.Close();
    }
    #endregion

    

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
