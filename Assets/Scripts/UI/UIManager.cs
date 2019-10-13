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
}
