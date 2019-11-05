using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{   
    [SerializeField] private GameObject loadingObject;
    [SerializeField] private TextMeshProUGUI loadingMessage;
    [SerializeField] private GameObject loadingAnim;

    public GameObject sleepMode;

    public UIPopup loginPopup;
    public TMPro.TMP_InputField username;
    public TMPro.TMP_InputField password;
    public bool rememberMe;

    public bool isLoggedin;
    public bool tryingToLogin;

    public TextMeshProUGUI atkScore;
    public TextMeshProUGUI atkCombo;
    public TextMeshProUGUI atkTimer;

    public TextMeshProUGUI defBlocks;
    public TextMeshProUGUI defTimer;
    public UIImage defMeter;

    public TextMeshProUGUI spdBounces;
    public TextMeshProUGUI spdScore;
    public TextMeshProUGUI spdTimer;
    public UIImage[] spdIndicators;

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

    public void SetLoading(bool active, string message, bool animation)
    {
        loadingObject.SetActive(active);
        loadingMessage.text = message;
        loadingAnim.SetActive(animation);
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

    // public void Feed(int hungerChange)
    // {
    //     GameManager.instance.activePets[0].Feed(hungerChange, 10);
    // }

    public void Train(string stat)
    {
        GameManager.instance.StartTraining(0, stat);
    }

    public void Praise()
    {
        GameManager.instance.Praise();
    }

    public void Scold()
    {
        GameManager.instance.Scold();
    }

    public void TurnLight(bool on)
    {
        sleepMode.SetActive(!on);
    }
}
