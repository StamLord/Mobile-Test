using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class StatPopup : MonoBehaviour
{   
    public TextMeshProUGUI statGain;
    public UIView window;
    
    public void Popup(string stat, int gain)
    {
        statGain.text = stat + " +" + gain;
        window.Show();
    }
}
