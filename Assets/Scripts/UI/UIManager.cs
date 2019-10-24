using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{   
    
    public UIPopup loginPopup;
    public TMPro.TMP_InputField username;
    public TMPro.TMP_InputField password;
    public bool rememberMe;

    public bool isLoggedin;
    public bool tryingToLogin;

    public TextMeshProUGUI atkScore;
    public TextMeshProUGUI atkCombo;
    public TextMeshProUGUI atkTimer;

    #region Singleton

    public static UIManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("More than 1 instance of UIManager exists. Destroying: " + this.gameObject);
            Destroy(gameObject);
        }
    }

    #endregion

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
        isLoggedin = DataService.isLoggedin;
        tryingToLogin = DataService.tryingToLogin;

        if(DataService.isLoggedin == false 
        && DataService.tryingToLogin == false 
        && loginPopup.IsHidden){
            // Pop Login Window
            loginPopup.Show();
        }
    }

    public void Login()
    {
        StartCoroutine(GameManager.instance.Login(username.text, password.text, rememberMe));
        loginPopup.Hide();
    }

    public void RememberMe()
    {
        rememberMe = !rememberMe;
    }

    public void Feed(int hungerChange)
    {
        GameManager.instance.activePets[0].Feed(hungerChange, 10);
    }

    public void Train(string stat)
    {
        GameManager.instance.StartTraining(0, stat);
    }
}
