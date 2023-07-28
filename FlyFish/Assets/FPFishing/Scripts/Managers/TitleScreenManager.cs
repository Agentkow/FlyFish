using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loadButton;

    private void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/Save.dat"))
        {
            loadButton.SetActive(true);
        }
        else
        {
            loadButton.SetActive(false);
        }
    }

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
