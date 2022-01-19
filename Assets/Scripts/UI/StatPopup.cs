using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class StatPopup : MonoBehaviour
{   
    public TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    [SerializeField] private UIView view;
    
    public void Popup(string stat, int gain)
    {
        text.text = stat;
        if(gain > 0) text.text += "+";
        text.text += gain;
        if(view)
            view.Show();
        else if(animator)
            animator.SetTrigger("Popup");
    }

    public void Popup(string text)
    {
        this.text.text = text;
        if(view)
            view.Show();
        else if(animator)
            animator.SetTrigger("Popup");
    }
}
