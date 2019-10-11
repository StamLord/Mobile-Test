using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class UIManager : MonoBehaviour
{   
    
    public UIPopup loginPopup;

    void Update(){
        if(DataService.isLoggedin == false && loginPopup.IsHidden){
            // Pop Login Window
            loginPopup.Show();
        }
    }
}
