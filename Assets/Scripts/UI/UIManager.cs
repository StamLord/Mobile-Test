using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{   
    
    public UIPopup loginPopup;
    public TMPro.TMP_InputField username;
    public TMPro.TMP_InputField password;

    void Start()
    {
        string s_username = PlayerPrefs.GetString("username");
        string s_password = PlayerPrefs.GetString("password");

        if(string.IsNullOrEmpty(s_username) == false)
            username.text = s_username;

        if(string.IsNullOrEmpty(s_password) == false)
            password.text = s_password;
    }

    void Update(){
        if(DataService.isLoggedin == false 
        && DataService.tryingToLogin == false 
        && loginPopup.IsHidden){
            // Pop Login Window
            loginPopup.Show();
        }
    }

    public void Login()
    {
        StartCoroutine(DataService.Login(username.text, password.text));
        loginPopup.Hide();
    }

    public void Feed(int hungerChange)
    {
        GameManager.instance.activePets[0].Feed(hungerChange, 10);
    }
}
