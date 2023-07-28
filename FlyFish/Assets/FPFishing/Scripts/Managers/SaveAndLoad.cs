using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveAndLoad : MonoBehaviour
{
    private static SaveAndLoad _save;
    public static SaveAndLoad save { get { return _save; } }

    [SerializeField] private int itemLimit = 3;
    [field: Header("Save info holder")]
    List<bool> getHooks;
    List<bool> getPoles;
    List<bool> getLicenses;
    int money;

    private void Start()
    {

    }
    #region get saved values
    public List<bool> getHooklist()
    {
        return getHooks;
    }
    public List<bool> getPolelist()
    {
        return getPoles;
    }
    public List<bool> getLicenselist()
    {
        return getLicenses;
    }
    public int getMoney()
    {
        return money;
    }
    #endregion

    public void StartNewGame()
    {
        getHooks = new List<bool>();
        for (int i = 0; i < GameManager.gm.hooks.Count; i++)
        {
            getHooks.Add(false);
        }
        for (int i = 0; i < GameManager.gm.poles.Count; i++)
        {
            getPoles.Add(false);
        }
        for (int i = 0; i < GameManager.gm.fishLicense.Count; i++)
        {
            getLicenses.Add(false);
        }
        money = 0;
    }

    #region Save & Load
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/Save.dat"))
        {
            //create binary formater
            BinaryFormatter bf = new BinaryFormatter();

            //open file
            FileStream file = File.Open(Application.persistentDataPath + "/Save.dat", FileMode.Open);

            //deserialize the data for reading
            PlayerData data = (PlayerData)bf.Deserialize(file);

            //close the file.
            file.Close();
            //inputs the saved data
            //List all variables you want to load here in this format: variable = data.variable;
            getHooks = data.hooks;
            getPoles = data.poles;
            getLicenses = data.licenses;


        }
        else
        {
            
        }
    }

    public void Save(List<bool> inputHooks, List<bool> inputPoles, List<bool> inputLicenses, int inputMoney)
    {
        //create binary formater
        BinaryFormatter bf = new BinaryFormatter();

        //create a file
        FileStream file = File.Create(Application.persistentDataPath + "/Save.dat");

        // creates container for my objects to save
        PlayerData data = new PlayerData();
        //list all variables you wish to save in the following format data.variable = variable;
        data.hooks = inputHooks;
        data.poles = inputPoles;
        data.licenses = inputLicenses;
        data.money = inputMoney;

        //writes my data onto the file
        bf.Serialize(file, data);

        //closes file
        file.Close();
    }
    #endregion
}

[Serializable]
class PlayerData
{
    //Declare all variables to save here. All variables must be declared as public. I.E. public int iScore;
    public List<bool> hooks, poles, licenses;
    public int money;
}
