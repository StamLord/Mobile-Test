using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class StatPopup : MonoBehaviour
{   
    public int popupDuration = 3;
    public string stat = "ATK";
    public TextMeshProUGUI statGain;
    private UIView window;

    void Start()
    {
        window = GetComponent<UIView>();
    }

    void Popup(int gain)
    {
        statGain.text = stat + " +" + gain;
        window.Show();

        StartCoroutine(EndMinigame());
    }

    IEnumerator EndMinigame()
    {
        yield return new WaitForSeconds(popupDuration);
        window.Hide();
    }

}
