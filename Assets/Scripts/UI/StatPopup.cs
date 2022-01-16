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
        statGain.text = stat;
        if(gain > 0) statGain.text += "+";
        statGain.text += gain;
        window.Show();
    }

    public void Popup(string text)
    {
        statGain.text = text;
        window.Show();
    }
}
