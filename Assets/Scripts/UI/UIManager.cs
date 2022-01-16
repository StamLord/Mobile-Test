﻿using UnityEngine;
using Doozy.Engine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{   
    [Header("Loading")]
    [SerializeField] private GameObject loadingObject;
    [SerializeField] private TextMeshProUGUI loadingMessage;
    [SerializeField] private GameObject loadingAnim;
    [Space]

    public GameObject sleepMode;

    [Header("Login Window")]
    public UIPopup loginPopup;
    public TMPro.TMP_InputField username;
    public TMPro.TMP_InputField password;
    public bool rememberMe;

    public bool isLoggedin;
    public bool tryingToLogin;

    [Header("Register Window")]
    public UIPopup registerPopup;
    public TMPro.TMP_InputField regUsername;
    public TMPro.TMP_InputField regPassword;
    public TMPro.TMP_InputField regEmail;

    public TextMeshProUGUI lightLevel;
    
    [Header("Stat Cards")]
    public UIView statCard1;
    public UIStatCard sc1;
    [Space]

    [Header("Training")]
    public TextMeshProUGUI atkScore;
    public TextMeshProUGUI atkCombo;
    public TextMeshProUGUI atkTimer;
    [Space]
    public TextMeshProUGUI defBlocks;
    public TextMeshProUGUI defTimer;
    public UIImage defMeter;
    [Space]
    public TextMeshProUGUI spdBounces;
    public TextMeshProUGUI spdScore;
    public TextMeshProUGUI spdTimer;
    public UIImage[] spdIndicators;

    [Header("Egg Window")]
    [SerializeField] private UIButton eggButton;

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

        SensorsManager.onLuxChange += UpdateLightLevel;
        GameManager.onActivePetsChange += UpdatePets;
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

    public void SetLoading(bool active, string message, bool animation)
    {
        loadingObject.SetActive(active);
        loadingMessage.text = message;
        loadingAnim.SetActive(animation);
    }

    public void Login()
    {
        StartCoroutine(this.LoginEnum(username.text, password.text, rememberMe));
    }

    private IEnumerator LoginEnum(string username, string password, bool rememberMe)
    {
        Coroutine<bool> routine = this.StartCoroutine<bool>(GameManager.instance.Login(username, password, rememberMe));
        yield return routine.coroutine;

        Debug.Log(routine.returnVal);
        if(routine.returnVal == true)
            loginPopup.Hide();
    }

    public void ShowRegisterPopup()
    {
        registerPopup.Show();
    }

    public void Register()
    {
        StartCoroutine(this.RegisterEnum(regUsername.text, regPassword.text, regEmail.text));
    }

    private IEnumerator RegisterEnum(string username, string password, string email)
    {
        Coroutine<bool> routine = this.StartCoroutine<bool>(GameManager.instance.Register(username, password, email));
        yield return routine.coroutine;

        Debug.Log(routine.returnVal);
        if(routine.returnVal == true)
            registerPopup.Hide();
            loginPopup.Hide();
    }

    public void RememberMe()
    {
        rememberMe = !rememberMe;
    }

    public void Train(string stat)
    {
        GameManager.instance.StartTraining(stat);
    }

    public void Praise()
    {
        GameManager.instance.Praise();
    }

    public void Scold()
    {
        GameManager.instance.Scold();
    }

    public void TurnLight(bool wake)
    {
        //sleepMode.SetActive(!on);
        if(wake)
            GameManager.instance.Wake();
        else
            GameManager.instance.Sleep();
    }

    private void UpdateLightLevel(float lux)
    {
        lightLevel.text = "Light: " + lux;
    }

    public void ShowStatCards()
    {
        GameManager gm = GameManager.instance;

        if(gm.activePets.Count > 0 && gm.activePets[0] != null)
        {
            sc1.UpdateCard(0);
            statCard1.enabled = true;
            statCard1.Show();
        }
    }

    public void HideStatCards()
    {
        statCard1.Hide();
    }

    private void UpdatePets(List<ActivePet> activePets)
    {
        // In case no active pet is present, call "Eggs" window

        if(activePets.Count == 0)
            eggButton.ExecuteClick();

    }
}
