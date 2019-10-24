using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class StatPopup : MonoBehaviour
{   
    public int popupDuration = 3;
    public TextMeshProUGUI statGain;
    public UIView window;
    
    public void Popup(string stat, int gain)
    {
        statGain.text = stat + " +" + gain;
        window.Show();

        //StartCoroutine(HidePopup());
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(popupDuration);
        window.Hide();
    }

}
