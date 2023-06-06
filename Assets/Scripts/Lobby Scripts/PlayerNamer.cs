using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    

    public static string DisplayName { get; private set; }

    private const string PlayerDefaultName = "Hi name, I'm Dad";

    private void Start()
    {
        if (PlayerPrefs.HasKey(PlayerDefaultName)) 
        {
            string defaultName = PlayerPrefs.GetString(PlayerDefaultName);

            //nameInputField.text = defaultName;

            SetPlayerName(defaultName);
        }

    }


    public void SetPlayerName(string name)
    {
        //continueButton.interactible = !IsNameValid(name);
    }
    
    public bool IsNameValid(string name)
    {

        //If we want to add name censorshup, do it here

        if (!string.IsNullOrEmpty(name))
        { return false; }

        return true;
    }

    public void SavePlayerName()
    {
        //DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerDefaultName, DisplayName);
    }

}
