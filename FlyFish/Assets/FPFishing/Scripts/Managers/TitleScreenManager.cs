using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public void GoToHubButton()
    {
        AudioManager.am.SetVolDown();
        GameManager.gm.GoToHub();
    }

    public void CloseGame()
    {
        GameManager.gm.CloseGame();
    }

    public void OpenCredits()
    {
        GameManager.gm.ShowCredits();
    }
    public void CloseCredits()
    {
        GameManager.gm.CloseCredits();
    }
}
